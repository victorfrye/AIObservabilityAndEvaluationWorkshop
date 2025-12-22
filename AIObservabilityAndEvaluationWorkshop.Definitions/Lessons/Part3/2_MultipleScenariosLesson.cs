using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(3, 2, "Multiple Scenarios", needsInput: true)]
public class MultipleScenariosLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Multiple Scenarios placeholder. Input: {message}");
    }
}
