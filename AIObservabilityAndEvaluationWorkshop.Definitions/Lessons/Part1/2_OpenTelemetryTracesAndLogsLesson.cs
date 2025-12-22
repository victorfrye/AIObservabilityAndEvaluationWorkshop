using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(1, 2, "OpenTelemetry Traces and Logs", needsInput: false)]
public class OpenTelemetryTracesAndLogsLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult("Placeholder for OpenTelemetry Traces and Logs");
    }
}
