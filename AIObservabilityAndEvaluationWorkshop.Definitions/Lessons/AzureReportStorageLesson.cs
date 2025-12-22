using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(3, 6, "Azure report storage", needsInput: true)]
public class AzureReportStorageLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Azure report storage placeholder. Input: {message}");
    }
}
