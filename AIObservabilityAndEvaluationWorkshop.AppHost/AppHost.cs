#pragma warning disable ASPIREINTERACTION001 // Interaction Service is for evaluation purposes only
using Aspire.Hosting;
using Aspire.Hosting.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add the console app project (without WithExplicitStart so it doesn't auto-start)
var consoleApp = builder.AddProject<AIObservabilityAndEvaluationWorkshop_ConsoleRunner>("console-app");

// Subscribe to AfterResourcesCreatedEvent to prompt for input after dashboard is ready
builder.Eventing.Subscribe<AfterResourcesCreatedEvent>(async (@event, cancellationToken) =>
{
    var interactionService = @event.Services.GetRequiredService<IInteractionService>();

    if (!interactionService.IsAvailable)
    {
        return;
    }

    // Prompt the user for input
    var result = await interactionService.PromptInputAsync(
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
    
    // Show a notification with the message
    await interactionService.PromptNotificationAsync("Input Received", 
        $"You entered: {message}. " +
        "You can now start the console-app resource from the dashboard, and it will use this message.");
    
    // Note: To actually execute the console app programmatically, you would need to
    // use a custom resource command. For now, the user can start it manually from the dashboard.
});

builder.Build().Run();
