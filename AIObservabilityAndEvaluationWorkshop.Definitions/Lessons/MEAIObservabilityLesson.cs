using AIObservabilityAndEvaluationWorkshop.Definitions;
using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(1, 5, "MEAI Observability", needsInput: true)]
public class MEAIObservabilityLesson : LessonBase
{
    protected override async Task<string> RunAsync(string message)
    {
        return $"Processed: {message}";
    }
}
