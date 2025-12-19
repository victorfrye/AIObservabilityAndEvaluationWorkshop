#pragma warning disable ASPIREINTERACTION001 // Interaction Service is for evaluation purposes only

using Microsoft.Extensions.DependencyInjection;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add the console app project (without WithExplicitStart so it doesn't auto-start)
IResourceBuilder<ProjectResource> consoleAppBuilder =
    builder.AddProject<AIObservabilityAndEvaluationWorkshop_ConsoleRunner>("console-app")
        //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:19288")
        .WithExplicitStart();

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

    // Ensure the console app has the OTLP endpoint for telemetry
    consoleAppBuilder = consoleAppBuilder.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:19288");

    // Configure the console app to run with the display command and message
    consoleAppBuilder.WithArgs("display", message);

    // Show a notification with the message
    await interactionService.PromptNotificationAsync("Input Received",
        $"You entered: '{message}'. The app can now be started from the dashboard.", new NotificationInteractionOptions()
        {
            Intent = MessageIntent.Information,
            ShowSecondaryButton = false,
        });
});

builder.Build().Run();
