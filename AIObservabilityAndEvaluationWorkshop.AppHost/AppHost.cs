#pragma warning disable ASPIREINTERACTION001 // Interaction Service is for evaluation purposes only

using AIObservabilityAndEvaluationWorkshop.Definitions;
using Microsoft.Extensions.DependencyInjection;
using Projects;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AIObservabilityAndEvaluationWorkshop.AppHost;

internal partial class Program
{
    private static void Main(string[] args)
    {
        IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

        string[] appArgs = [];

        // Add the console app project (without WithExplicitStart so it doesn't auto-start)
        IResourceBuilder<ProjectResource> consoleAppBuilder =
            builder.AddProject<AIObservabilityAndEvaluationWorkshop_ConsoleRunner>("console-app")
                //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:19288")
                .WithExplicitStart()
                .WithOutputWatcher(GetConsoleResultRegex(), isSecret: false, "json")
                .OnMatched(async (e, ct) =>
                {
                    Console.WriteLine($"AppHost: OnMatched event fired for {e.Resource.Name}: {e.Key}, {e.Message}");
                    
                    // Get the captured JSON from the regex match
                    if (e.Properties.TryGetValue("json", out var jsonValue))
                    {
                        string json = jsonValue.ToString()!;
                        Console.WriteLine($"AppHost: Captured JSON: {json}");
                        
                        try
                        {
                            // Deserialize the JSON result
                            ConsoleResult? result = JsonSerializer.Deserialize<ConsoleResult>(json);
                            if (result != null)
                            {
                                // Get interaction service to show notification
                                IInteractionService? interactionSvc = e.ServiceProvider.GetService<IInteractionService>();
                                
                                if (interactionSvc is { IsAvailable: true })
                                {
                                    switch (result.Success)
                                    {
                                        case true when !string.IsNullOrWhiteSpace(result.Output):
                                            await interactionSvc.PromptNotificationAsync(
                                                title: "Completed",
                                                message: result.Output,
                                                options: new NotificationInteractionOptions
                                                {
                                                    Intent = MessageIntent.Success,
                                                    ShowSecondaryButton = false
                                                }, cancellationToken: ct);
                                            break;
                                        case true when string.IsNullOrWhiteSpace(result.Output):
                                            await interactionSvc.PromptNotificationAsync(
                                                title: "No Output",
                                                message: "The operation completed, but produced no output",
                                                options: new NotificationInteractionOptions
                                                {
                                                    Intent = MessageIntent.Warning,
                                                    ShowSecondaryButton = false
                                                }, cancellationToken: ct);
                                            break;
                                        default:
                                            await interactionSvc.PromptNotificationAsync(
                                                title: "Error",
                                                message: result.ErrorMessage ?? "An unknown error occurred.",
                                                options: new NotificationInteractionOptions
                                                {
                                                    Intent = MessageIntent.Error,
                                                    ShowSecondaryButton = false
                                                }, cancellationToken: ct);
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("AppHost: Interaction service not available, skipping notification");
                                }
                            }
                            else
                            {
                                Console.WriteLine("AppHost: Failed to deserialize console result");
                            }
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"AppHost: JSON deserialization failed: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("AppHost: No JSON captured from regex match");
                    }
                })
                .WithArgs(context =>
                {
                    context.Args.Clear();
                    foreach (string arg in appArgs)
                    {
                        context.Args.Add(arg);
                    }
                });

        // Add a custom command that prompts for input and starts the resource
        consoleAppBuilder.WithCommand("start-with-input", "Start with Input", async (context) =>
        {
            IInteractionService interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();

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

            Console.WriteLine($"AppHost: Starting console app with args: {string.Join(", ", appArgs)}");

            // Simply start the resource
            ResourceCommandService commandService = context.ServiceProvider.GetRequiredService<ResourceCommandService>();
            return await commandService.ExecuteCommandAsync(context.ResourceName, "resource-start");
        },
        new CommandOptions
        {
            Description = "Configure the console app with user input and start it",
            IconName = "Play",
            IsHighlighted = true
        });

        builder.Build().Run();
    }

    [GeneratedRegex(@"^CONSOLE_RESULT:\s*(?<json>.*)$", RegexOptions.Multiline)]
    public static partial Regex GetConsoleResultRegex();
}
