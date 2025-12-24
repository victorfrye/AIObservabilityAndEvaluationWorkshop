using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 26, "Contextful evaluators", needsInput: true,
    informationalScreenTitle: "Contextful Evaluators",
    informationalScreenMessage: "This lesson demonstrates how to use evaluators with additional context, such as conversation history, source documents, or other relevant information to provide more accurate evaluations.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Contextful Evaluators - Message Input",
    inputPromptMessage: "Enter a message to evaluate with context:")]
public class ContextfulEvaluatorsLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"Contextful evaluators placeholder. Input: {message}");
    }
}

