using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 1, "Simple evaluator in action", needsInput: true)]
public class SimpleEvaluatorLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Simple evaluator placeholder. Input: {message}");
    }
}
