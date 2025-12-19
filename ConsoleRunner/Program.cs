// The console app receives the user input as a command-line argument from the AppHost.
// The AppHost uses IInteractionService to prompt the user, then passes the result here.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Diagnostics;

var builder = Host.CreateApplicationBuilder(args);

// Add Aspire service defaults for telemetry
builder.AddServiceDefaults();

var activitySource = new ActivitySource(builder.Environment.ApplicationName);

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("ConsoleRunner started");

// Define output file path
var outputFilePath = Path.Combine(AppContext.BaseDirectory, "console_output.txt");

// Create the root command
var rootCommand = new RootCommand("Console application for displaying messages");

// Add the display command
var displayCommand = new Command("display", "Display a message");
var messageArgument = new Argument<string>("message", "The message to display");
displayCommand.AddArgument(messageArgument);

displayCommand.SetHandler(async (string message) =>
{
    var commandLogger = host.Services.GetRequiredService<ILogger<Program>>();

    using var activity = activitySource.StartActivity("DisplayMessage");
    activity?.SetTag("message", message);

    commandLogger.LogInformation("Displaying message: {Message}", message);

    Console.WriteLine(message);

    // Write to output file
    try
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var outputLine = $"[{timestamp}] {message}";
        File.AppendAllText(outputFilePath, outputLine + Environment.NewLine);
    }
    catch (Exception ex)
    {
        commandLogger.LogError(ex, "Failed to write to output file");
    }
}, messageArgument);

rootCommand.AddCommand(displayCommand);

await host.StartAsync();

return await rootCommand.InvokeAsync(args);