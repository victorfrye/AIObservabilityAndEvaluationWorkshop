// The console app receives the user input as a command-line argument from the AppHost.
// The AppHost uses IInteractionService to prompt the user, then passes the result here.
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.Diagnostics;

var builder = Host.CreateApplicationBuilder(args);

// Add Aspire service defaults for telemetry
builder.AddServiceDefaults();

// Configure telemetry
var activitySource = new ActivitySource(builder.Environment.ApplicationName);

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
    using var activity = activitySource.StartActivity("DisplayMessage");
    activity?.SetTag("message", message);
    activity?.SetTag("operation", "display");

    // Write to console (for local debugging)
    Console.WriteLine(message);

    // Write to shared output file for AppHost to read
    try
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var outputLine = $"[{timestamp}] {message}";
        File.AppendAllText(outputFilePath, outputLine + Environment.NewLine);

        activity?.SetTag("output_file_written", true);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error writing to output file: {ex.Message}");
        activity?.SetTag("output_file_error", ex.Message);
    }
}, messageArgument);

rootCommand.AddCommand(displayCommand);

// Build the host
var host = builder.Build();
await host.StartAsync();

// Parse and execute commands
return await rootCommand.InvokeAsync(args);