using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.NLP;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 21, "BLEU Evaluator", needsInput: true,
    informationalScreenTitle: "BLEU Evaluator",
    informationalScreenMessage: "This lesson demonstrates the BLEU (Bilingual Evaluation Understudy) Evaluator, which measures the similarity between generated text and reference text using n-gram precision. It's commonly used for machine translation evaluation.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "BLEU Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate using BLEU score:")]
public class BLEUEvaluatorLesson(IChatClient chatClient, ILogger<BLEUEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        // First, get a response from the chat client
        var response = await chatClient.GetResponseAsync([new ChatMessage(ChatRole.User, message)]);
        
        BLEUEvaluator evaluator = new();
        
        // BLEU evaluator compares a response to reference responses
        // For this lesson, we'll use the response as both the response and a simple reference
        // In practice, you would have actual reference responses to compare against
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            [new ChatMessage(ChatRole.User, message)],
            response);

        return evaluationResult;
    }
}

