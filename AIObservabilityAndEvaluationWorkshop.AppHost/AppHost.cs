#pragma warning disable ASPIREINTERACTION001 // Interaction Service is for evaluation purposes only

using Microsoft.Extensions.DependencyInjection;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add the console app project (without WithExplicitStart so it doesn't auto-start)
IResourceBuilder<ProjectResource> consoleAppBuilder = 
    builder.AddProject<AIObservabilityAndEvaluationWorkshop_ConsoleRunner>("console-app")
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

    // Configure the console app to run with the display command and message
    consoleAppBuilder.WithArgs("display", message);

    // Show a notification with the message
    await interactionService.PromptNotificationAsync("Input Received",
        $"You entered: '{message}'. The app can now be started from the dashboard.", new NotificationInteractionOptions()
        {
            Intent = MessageIntent.Information,
            ShowSecondaryButton = false,
        });

    // Set up file monitoring for ConsoleRunner output
    var outputFilePath = Path.Combine(AppContext.BaseDirectory, "console_output.txt");
    var fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(outputFilePath)!, Path.GetFileName(outputFilePath))
    {
        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
    };

    // Keep track of the last read position to only show new content
    long lastReadPosition = 0;

    fileWatcher.Changed += async (sender, e) =>
    {
        try
        {
            // Small delay to ensure file is fully written
            await Task.Delay(100);

            using var stream = new FileStream(outputFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);

            // Seek to the last read position
            stream.Seek(lastReadPosition, SeekOrigin.Begin);

            var newContent = await reader.ReadToEndAsync();
            lastReadPosition = stream.Position;

            if (!string.IsNullOrWhiteSpace(newContent))
            {
                // Extract the latest message (last line)
                var lines = newContent.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 0)
                {
                    var latestLine = lines.Last().Trim();
                    if (!string.IsNullOrEmpty(latestLine))
                    {
                        await interactionService.PromptNotificationAsync(
                            "Console Output",
                            $"ConsoleRunner output: {latestLine}",
                            new NotificationInteractionOptions()
                            {
                                Intent = MessageIntent.Information,
                                ShowSecondaryButton = false,
                            });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await interactionService.PromptNotificationAsync(
                "File Monitoring Error",
                $"Error reading console output: {ex.Message}",
                new NotificationInteractionOptions()
                {
                    Intent = MessageIntent.Warning,
                    ShowSecondaryButton = false,
                });
        }
    };

    fileWatcher.EnableRaisingEvents = true;
});

builder.Build().Run();
