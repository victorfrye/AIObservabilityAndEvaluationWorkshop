using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(3, 9, "EXERCISE 5", needsInput: true)]
public class Exercise5Lesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Exercise 5 placeholder. Input: {message}");
    }
}
