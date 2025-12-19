#pragma warning disable ASPIREINTERACTION001 // Interaction Service is for evaluation purposes only
using Aspire.Hosting;
using Aspire.Hosting.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add the console app project (without WithExplicitStart so it doesn't auto-start)
var consoleAppBuilder = builder.AddProject<AIObservabilityAndEvaluationWorkshop_ConsoleRunner>("console-app");

// Subscribe to AfterResourcesCreatedEvent to prompt for input after dashboard is ready
builder.Eventing.Subscribe<AfterResourcesCreatedEvent>(async (@event, cancellationToken) =>
{
    var interactionService = @event.Services.GetRequiredService<IInteractionService>();

    if (!interactionService.IsAvailable)
    {
        return;
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
        await interactionService.PromptNotificationAsync("Cancelled", "User canceled the input.");
        return;
    }

    var message = result.Data?.Value ?? "Hello, World!";

    // Launch the console app with the message as command line arguments
    try
    {
        // Construct the path to the console app DLL
        var appHostDir = AppContext.BaseDirectory;
        var solutionDir = Path.GetFullPath(Path.Combine(appHostDir, "..", "..", ".."));
        var consoleAppDll = Path.Combine(solutionDir, "ConsoleRunner", "bin", "Debug", "net10.0", "AIObservabilityAndEvaluationWorkshop.ConsoleRunner.dll");

        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{consoleAppDll}\" --message \"{message}\"",
            UseShellExecute = true
        };

        System.Diagnostics.Process.Start(startInfo);
    }
    catch (Exception ex)
    {
        await interactionService.PromptNotificationAsync("Error",
            $"Failed to launch console app: {ex.Message}");
        return;
    }

    // Show a notification with the message
    await interactionService.PromptNotificationAsync("Input Received",
        $"You entered: {message}. " +
        "The console app has been launched with this message.");
});

builder.Build().Run();
