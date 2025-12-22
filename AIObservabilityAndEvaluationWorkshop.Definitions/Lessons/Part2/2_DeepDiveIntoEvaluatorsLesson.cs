using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 2, "Deep dive into evaluators", needsInput: false)]
public class DeepDiveIntoEvaluatorsLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult("Deep dive into evaluators placeholder");
    }
}
