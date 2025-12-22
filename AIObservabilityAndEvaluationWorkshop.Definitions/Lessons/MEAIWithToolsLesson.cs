using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(1, 6, "MEAI with Tools", needsInput: true)]
public class MEAIWithToolsLesson : LessonBase
{
    protected override async Task<string> RunAsync(string message)
    {
        return $"Processed: {message}";
    }
}
