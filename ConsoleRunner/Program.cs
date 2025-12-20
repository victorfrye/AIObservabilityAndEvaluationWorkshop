// The console app receives the user input as a command-line argument from the AppHost.
// The AppHost uses IInteractionService to prompt the user, then passes the result here.
using AIObservabilityAndEvaluationWorkshop.ConsoleRunner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Log OTEL configuration diagnostics before services are configured
TelemetryDiagnostics.LogPreConfigurationDiagnostics(builder);

builder.AddServiceDefaults();

// Note: DisplayCommand's ActivitySource is automatically registered via the wildcard pattern
// in ServiceDefaults (AIObservabilityAndEvaluationWorkshop.ConsoleRunner*)
// No need to call AddOpenTelemetry() again as that creates a separate builder

builder.Services.AddScoped<DisplayCommand>();
IHost host = builder.Build();

// Log telemetry status after services are configured
TelemetryDiagnostics.LogPostConfigurationDiagnostics(host, builder);

await host.StartAsync();

// Log ActivitySource diagnostics after host starts (TracerProvider is now built)
ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
TelemetryDiagnostics.LogActivitySourceDiagnosticsAfterStart(logger, builder.Environment.ApplicationName, host.Services);

// Add the display command that allows users to send an input in through a message parameter
Command displayCommand = new Command("display", "Display a message");
Argument<string> messageArgument = new Argument<string>("message", "The message to display");
displayCommand.AddArgument(messageArgument);

DisplayCommand command = host.Services.GetRequiredService<DisplayCommand>();
displayCommand.SetHandler(command.ExecuteAsync, messageArgument);

// Create the root command that routes inputs to other commands
RootCommand rootCommand = new RootCommand("Console application for displaying messages");
rootCommand.AddCommand(displayCommand);

int result = await rootCommand.InvokeAsync(args);

// For console apps, give telemetry time to be exported before exiting
// This ensures all telemetry is exported to Aspire before the app terminates
try
{
    logger.LogInformation("Waiting for telemetry export before app exit...");
    
    // Give the exporter time to send the data
    // OTLP exporters typically batch and send data asynchronously
    await Task.Delay(TimeSpan.FromSeconds(2));
    
    logger.LogInformation("Telemetry export wait completed");
}
catch (Exception ex)
{
    logger.LogWarning(ex, "Error during telemetry export wait");
}

return result;
