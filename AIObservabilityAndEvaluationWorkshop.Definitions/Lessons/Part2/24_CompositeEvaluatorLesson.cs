using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 24, "Composite evaluator", needsInput: true,
    informationalScreenTitle: "Composite Evaluator",
    informationalScreenMessage: "This lesson demonstrates how to combine multiple evaluators into a composite evaluator that assesses responses across multiple dimensions simultaneously.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Composite Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate using multiple evaluators:")]
public class CompositeEvaluatorLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Composite evaluator placeholder. Input: {message}");
    }
}
