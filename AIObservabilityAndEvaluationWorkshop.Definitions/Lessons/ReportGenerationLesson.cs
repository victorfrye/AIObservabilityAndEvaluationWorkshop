using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(3, 1, "Report Generation", needsInput: true)]
public class ReportGenerationLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Report Generation placeholder. Input: {message}");
    }
}
