using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(3, 5, "Custom report storage", needsInput: true)]
public class CustomReportStorageLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Custom report storage placeholder. Input: {message}");
    }
}
