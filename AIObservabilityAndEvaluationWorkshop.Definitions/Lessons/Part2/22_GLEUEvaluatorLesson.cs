using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.NLP;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 22, "GLEU Evaluator", needsInput: true,
    informationalScreenTitle: "GLEU Evaluator",
    informationalScreenMessage: "This lesson demonstrates the GLEU (Google's BLEU) Evaluator, an improved version of BLEU that provides better evaluation for text generation tasks by considering both precision and recall of n-grams.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "GLEU Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate using GLEU score:")]
public class GLEUEvaluatorLesson(IChatClient chatClient, ILogger<GLEUEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        // First, get a response from the chat client
        var response = await chatClient.GetResponseAsync([new ChatMessage(ChatRole.User, message)]);
        
        GLEUEvaluator evaluator = new();
        
        // GLEU evaluator compares a response to reference responses
        // For this lesson, we'll use the response as both the response and a simple reference
        // In practice, you would have actual reference responses to compare against
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            [new ChatMessage(ChatRole.User, message)],
            response);

        return evaluationResult;
    }
}

