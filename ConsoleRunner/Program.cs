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

// Explicitly register DisplayCommand's ActivitySource with OpenTelemetry
// This extends the existing OpenTelemetry configuration from AddServiceDefaults
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        // Register the ActivitySource used by DisplayCommand
        // The full type name is: AIObservabilityAndEvaluationWorkshop.ConsoleRunner.DisplayCommand
        string displayCommandSourceName = typeof(DisplayCommand).FullName!;
        tracing.AddSource(displayCommandSourceName);
    });

builder.Services.AddScoped<DisplayCommand>();
IHost host = builder.Build();

// Log telemetry status after services are configured
TelemetryDiagnostics.LogPostConfigurationDiagnostics(host, builder);

await host.StartAsync();

// Log ActivitySource diagnostics after host starts (TracerProvider is now built)
ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
TelemetryDiagnostics.LogActivitySourceDiagnosticsAfterStart(logger, builder.Environment.ApplicationName);

// Add the display command that allows users to send an input in through a message parameter
Command displayCommand = new Command("display", "Display a message");
Argument<string> messageArgument = new Argument<string>("message", "The message to display");
displayCommand.AddArgument(messageArgument);

DisplayCommand command = host.Services.GetRequiredService<DisplayCommand>();
displayCommand.SetHandler(command.ExecuteAsync, messageArgument);

// Create the root command that routes inputs to other commands
RootCommand rootCommand = new RootCommand("Console application for displaying messages");
rootCommand.AddCommand(displayCommand);

return await rootCommand.InvokeAsync(args);
