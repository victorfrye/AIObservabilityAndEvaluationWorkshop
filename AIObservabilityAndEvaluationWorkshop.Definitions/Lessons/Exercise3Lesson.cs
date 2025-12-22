using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 10, "EXERCISE 3", needsInput: true)]
public class Exercise3Lesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Exercise 3 placeholder. Input: {message}");
    }
}
