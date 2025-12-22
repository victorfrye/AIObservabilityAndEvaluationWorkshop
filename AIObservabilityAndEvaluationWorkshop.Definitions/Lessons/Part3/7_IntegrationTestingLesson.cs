using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(3, 7, "Integration Testing", needsInput: true)]
public class IntegrationTestingLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Integration Testing placeholder. Input: {message}");
    }
}
