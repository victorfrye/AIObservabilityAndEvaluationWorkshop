using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIObservabilityAndEvaluationWorkshop.ConsoleRunner;

public class DisplayCommand(ILogger<DisplayCommand> logger)
{
    private readonly ActivitySource _activitySource = new(typeof(DisplayCommand).FullName!);

    public Task ExecuteAsync(string message, string lessonId)
    {
        logger.LogInformation("Displaying message: {Message} for lesson: {LessonId}", message, lessonId);

        // Create service instance with the input message
        var aspireService = new AspireService(_activitySource, logger, message);

        // Log the console result using the helper
        aspireService.LogSuccess(
            message,
            lessonId,
            "DisplayCommand.Execute",
            ("message", message),
            ("lesson.id", lessonId),
            ("command.name", "display")
        );

        return Task.CompletedTask;
    }
}
