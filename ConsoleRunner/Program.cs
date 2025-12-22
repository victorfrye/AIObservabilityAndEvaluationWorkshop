using AIObservabilityAndEvaluationWorkshop.ConsoleRunner;
using AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;
using AIObservabilityAndEvaluationWorkshop.Definitions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddScoped<ExecuteLessonCommand>();

string ollamaEndpoint = builder.Configuration.GetConnectionString("llama3.2") 
                     ?? builder.Configuration.GetConnectionString("ollama") 
                     ?? "http://localhost:11434";

Console.WriteLine($"[DEBUG_LOG] Resolved ollamaEndpoint: '{ollamaEndpoint}'");

if (!Uri.TryCreate(ollamaEndpoint, UriKind.Absolute, out var ollamaUri))
{
    Console.WriteLine($"[DEBUG_LOG] Invalid URI '{ollamaEndpoint}', falling back to http://localhost:11434");
    ollamaUri = new Uri("http://localhost:11434");
}

builder.Services.AddChatClient(new Microsoft.Extensions.AI.OllamaChatClient(ollamaUri, "llama3.2"));

builder.Services.Scan(scan => scan
    .FromAssemblyOf<HelloWorkshop>()
    .AddClasses(classes => classes.AssignableTo<LessonBase>())
    .As<LessonBase>()
    .WithScopedLifetime());

IHost host = builder.Build();

// Log OTEL environment variables on startup
TelemetryDiagnostics.LogOtelEnvironmentVariables(host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(TelemetryDiagnostics)));

await host.StartAsync();

// Ensure the Ollama model is pulled before executing lessons
ILogger mainLogger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Program");
await EnsureOllamaModelAsync(ollamaUri, "llama3.2", mainLogger);

// Add the execute-lesson command that allows users to send an input in through a message parameter
 Command executeLessonCommand = new("execute-lesson", "Execute a lesson with a message");
Argument<string> messageArgument = new ("message", "The message to pass to the lesson");
Argument<string> lessonDisplayNameArgument = new ("lesson-display-name", "The lesson display name");
executeLessonCommand.AddArgument(messageArgument);
executeLessonCommand.AddArgument(lessonDisplayNameArgument);

ExecuteLessonCommand command = host.Services.GetRequiredService<ExecuteLessonCommand>();
executeLessonCommand.SetHandler(command.ExecuteAsync, messageArgument, lessonDisplayNameArgument);

// Create the root command that routes inputs to other commands
RootCommand rootCommand = new("Console application for running lessons");
rootCommand.AddCommand(executeLessonCommand);

// Add default handler for when no command is specified
rootCommand.SetHandler(async () =>
{
    ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DefaultCommand");
    ActivitySource activitySource = new(typeof(Program).FullName!);

    // Create service instance with null input for default command
    AspireService aspireService = new(activitySource, logger, null);

    // Log the console error using the helper
    aspireService.LogError(
        "Please use the 'Start with Input' command to run this application",
        null,
        ("command.name", "default")
    );

    await Task.CompletedTask;
});

int result = await rootCommand.InvokeAsync(args);

// For console apps, give telemetry time to be exported before exiting
// This ensures all telemetry is exported to Aspire before the app terminates
await Task.Delay(TimeSpan.FromSeconds(2));

return result;

async Task EnsureOllamaModelAsync(Uri baseUri, string modelName, ILogger logger)
{
    using HttpClient client = new HttpClient();
    client.BaseAddress = baseUri;
    client.Timeout = TimeSpan.FromMinutes(10); // pulling can take a long time

    int maxRetries = 5;
    for (int i = 1; i <= maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Checking if Ollama model '{modelName}' is available (attempt {i}/{maxRetries})...", modelName, i, maxRetries);
            
            // Check if model exists
            var tagsResponse = await client.GetAsync("/api/tags");
            if (tagsResponse.IsSuccessStatusCode)
            {
                var content = await tagsResponse.Content.ReadAsStringAsync();
                if (content.Contains(modelName))
                {
                    logger.LogInformation("Model '{modelName}' is already available.", modelName);
                    return;
                }
                
                logger.LogInformation("Model '{modelName}' not found. Pulling it now. This may take a few minutes...", modelName);
                var pullResponse = await client.PostAsJsonAsync("/api/pull", new { name = modelName });
                
                if (pullResponse.IsSuccessStatusCode)
                {
                    logger.LogInformation("Model '{modelName}' pulled successfully.", modelName);
                    return;
                }
                else
                {
                    string error = await pullResponse.Content.ReadAsStringAsync();
                    logger.LogWarning("Failed to pull model '{modelName}'. Status: {status}. Error: {error}", modelName, pullResponse.StatusCode, error);
                }
            }
            else
            {
                logger.LogWarning("Ollama server returned error status: {status}", tagsResponse.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning("Attempt {i} to connect to Ollama failed: {message}", i, ex.Message);
            if (i == maxRetries)
            {
                logger.LogError(ex, "Failed to connect to Ollama after {maxRetries} attempts.", maxRetries);
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}
