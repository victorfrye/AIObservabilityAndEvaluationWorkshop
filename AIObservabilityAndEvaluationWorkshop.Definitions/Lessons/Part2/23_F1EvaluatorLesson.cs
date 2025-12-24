using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.NLP;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 23, "F1 Evaluator", needsInput: true,
    informationalScreenTitle: "F1 Evaluator",
    informationalScreenMessage: "This lesson demonstrates the F1 Evaluator, which calculates the harmonic mean of precision and recall for text evaluation. It provides a balanced measure of accuracy.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "F1 Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate using F1 score:")]
public class F1EvaluatorLesson(IChatClient chatClient, ILogger<F1EvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        // First, get a response from the chat client
        var response = await chatClient.GetResponseAsync([new ChatMessage(ChatRole.User, message)]);
        
        F1Evaluator evaluator = new();
        
        // F1 evaluator compares a response to reference responses
        // For this lesson, we'll use the response as both the response and a simple reference
        // In practice, you would have actual reference responses to compare against
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            [new ChatMessage(ChatRole.User, message)],
            response);

        return evaluationResult;
    }
}

