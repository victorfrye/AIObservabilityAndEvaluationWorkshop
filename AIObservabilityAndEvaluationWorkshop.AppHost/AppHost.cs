#pragma warning disable ASPIREINTERACTION001 // Interaction Service is for evaluation purposes only

using Microsoft.Extensions.DependencyInjection;
using Projects;
using System.Text.Json;
using AIObservabilityAndEvaluationWorkshop.Definitions;

var builder = DistributedApplication.CreateBuilder(args);

// Create a temporary file for the console app output
var tempOutputFile = Path.GetTempFileName();
var tempOutputJsonFile = Path.ChangeExtension(tempOutputFile, ".json");

// Ensure the temporary JSON file doesn't exist initially
if (File.Exists(tempOutputJsonFile))
{
    File.Delete(tempOutputJsonFile);
}

string[] appArgs = [];

// Add the console app project (without WithExplicitStart so it doesn't auto-start)
IResourceBuilder<ProjectResource> consoleAppBuilder =
    builder.AddProject<AIObservabilityAndEvaluationWorkshop_ConsoleRunner>("console-app")
        .WithEnvironment("CONSOLE_OUTPUT_FILE", tempOutputJsonFile)
        //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:19288")
        .WithExplicitStart()
        .WithArgs(context =>
        {
            context.Args.Clear();
            foreach (var arg in appArgs)
            {
                context.Args.Add(arg);
            }
        });

// Add a custom command that prompts for input and starts the resource
consoleAppBuilder.WithCommand("start-with-input", "Start with Input", async (context) =>
{
    var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();

    if (!interactionService.IsAvailable)
    {
        return new ExecuteCommandResult { Success = false, ErrorMessage = "Interaction service not available" };
    }

    // Prompt the user for input
    InteractionResult<InteractionInput> result = await interactionService.PromptInputAsync(
        title: "User Input",
        message: "Please enter some text:",
        input: new InteractionInput
        {
            Name = "UserInput",
            InputType = InputType.Text,
            Required = true,
            Placeholder = "Enter your message here"
        });

    if (result.Canceled)
    {
        await interactionService.PromptNotificationAsync("Cancelled", "Operation cancelled by user.");
        return new ExecuteCommandResult { Success = false, ErrorMessage = "User cancelled input" };
    }

    appArgs = ["display", result.Data?.Value ?? "Hello, World!"];

    var commandService = context.ServiceProvider.GetRequiredService<ResourceCommandService>();
    var resourceNotificationService = context.ServiceProvider.GetRequiredService<ResourceNotificationService>();
    if (resourceNotificationService.TryGetCurrentState(context.ResourceName, out var state)
        && state.Snapshot.State?.Text == KnownResourceStates.NotStarted)
    {
        return await commandService.ExecuteCommandAsync(context.ResourceName, "resource-start");
    }

    return await commandService.ExecuteCommandAsync(context.ResourceName, "resource-restart");
},
new CommandOptions
{
    Description = "Configure the console app with user input and start it",
    IconName = "Play",
    IsHighlighted = true
});

// Subscribe to console app resource ready event to capture and display output after completion
consoleAppBuilder.OnResourceReady(async (resource, readyEvent, cancellationToken) =>
{
    var interactionService = readyEvent.Services.GetRequiredService<IInteractionService>();

    if (!interactionService.IsAvailable)
    {
        return;
    }

    // Use the temporary file we created for output
    var outputFilePath = tempOutputJsonFile;
    
    // Poll for the output file to be created and written
    var fileFound = false;
    var startTime = DateTime.UtcNow;

    // Poll every 100ms for up to 10 seconds
    while (!fileFound && (DateTime.UtcNow - startTime).TotalSeconds < 10)
    {
        if (File.Exists(outputFilePath))
        {
            try
            {
                // Check if file has content (not empty or just created)
                var fileInfo = new FileInfo(outputFilePath);
                if (fileInfo.Length > 0)
                {
                    // Additional small delay to ensure write is complete
                    await Task.Delay(200, cancellationToken);
                    fileFound = true;
                }
            }
            catch (IOException)
            {
                // File might still be locked, continue polling
            }
        }

        if (!fileFound)
        {
            await Task.Delay(100, cancellationToken);
        }
    }
    
    // Read and deserialize the result from the file
    ConsoleResult? result = null;
    try
    {
        if (File.Exists(outputFilePath))
        {
            // Retry reading in case file is still being written
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var jsonText = await File.ReadAllTextAsync(outputFilePath, cancellationToken);
                    result = JsonSerializer.Deserialize<ConsoleResult>(jsonText);
                    break;
                }
                catch (IOException)
                {
                    // File might still be locked, wait and retry
                    await Task.Delay(200, cancellationToken);
                }
                catch (JsonException ex)
                {
                    // JSON deserialization failed
                    result = new ConsoleResult
                    {
                        Success = false,
                        Input = "Unknown",
                        Output = null,
                        ErrorMessage = $"Failed to deserialize result: {ex.Message}"
                    };
                    break;
                }
            }
        }
    }
    catch (Exception ex)
    {
        // Log error but continue
        result = new ConsoleResult
        {
            Success = false,
            Input = "Unknown",
            Output = null,
            ErrorMessage = $"Error reading output file: {ex.Message}"
        };
    }

    // Display the result via appropriate notification
    if (result != null)
    {
        if (result.Success)
        {
            // Success notification with output
            await interactionService.PromptNotificationAsync(
                title: "Console App Success",
                message: result.Output ?? "Operation completed successfully.",
                options: new NotificationInteractionOptions
                {
                    Intent = MessageIntent.Success,
                    ShowSecondaryButton = false
                });
        }
        else
        {
            // Failure notification with error message
            await interactionService.PromptNotificationAsync(
                title: "Console App Error",
                message: result.ErrorMessage ?? "An unknown error occurred.",
                options: new NotificationInteractionOptions
                {
                    Intent = MessageIntent.Error,
                    ShowSecondaryButton = false
                });
        }
    }
    else
    {
        // If no result captured, show a notification indicating completion but no result
        await interactionService.PromptNotificationAsync(
            title: "Console App Completed",
            message: "The console app has completed execution, but no result was captured from the file.",
            options: new NotificationInteractionOptions
            {
                Intent = MessageIntent.Information,
                ShowSecondaryButton = false
            });
    }
});

builder.Build().Run();
