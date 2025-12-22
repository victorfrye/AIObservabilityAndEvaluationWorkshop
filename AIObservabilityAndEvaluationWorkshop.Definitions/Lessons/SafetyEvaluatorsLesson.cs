using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 6, "Safety Evaluators", needsInput: true)]
public class SafetyEvaluatorsLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Safety Evaluators placeholder. Input: {message}");
    }
}
