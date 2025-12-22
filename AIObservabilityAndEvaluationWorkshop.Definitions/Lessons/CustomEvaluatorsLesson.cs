using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 8, "Custom evaluators", needsInput: true)]
public class CustomEvaluatorsLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Custom evaluators placeholder. Input: {message}");
    }
}
