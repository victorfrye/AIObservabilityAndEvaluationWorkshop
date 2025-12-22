using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(3, 8, "Custom pass / fail", needsInput: true)]
public class CustomPassFailLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Custom pass / fail placeholder. Input: {message}");
    }
}
