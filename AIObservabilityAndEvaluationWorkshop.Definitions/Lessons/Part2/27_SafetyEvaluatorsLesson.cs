using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 27, "Safety Evaluators", needsInput: true,
    informationalScreenTitle: "Safety Evaluators",
    informationalScreenMessage: "This lesson demonstrates various safety evaluators that assess whether AI responses contain harmful, inappropriate, or unsafe content. These evaluators help ensure AI systems are safe to use.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Safety Evaluators - Message Input",
    inputPromptMessage: "Enter a message to evaluate for safety:")]
public class SafetyEvaluatorsLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Safety Evaluators placeholder. Input: {message}");
    }
}

