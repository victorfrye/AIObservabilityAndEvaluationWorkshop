using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(3, 3, "Trends over time", needsInput: true)]
public class TrendsOverTimeLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Trends over time placeholder. Input: {message}");
    }
}
