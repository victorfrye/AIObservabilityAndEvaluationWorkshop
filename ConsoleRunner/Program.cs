// The console app receives the user input as a command-line argument from the AppHost.
// The AppHost uses IInteractionService to prompt the user, then passes the result here.
using AIObservabilityAndEvaluationWorkshop.ConsoleRunner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddScoped<DisplayCommand>();
IHost host = builder.Build();

await host.StartAsync();

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
