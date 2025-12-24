using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Safety;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 17, "Violence Evaluator", needsInput: true,
    informationalScreenTitle: "Violence Evaluator",
    informationalScreenMessage: "This lesson demonstrates the Violence Evaluator, which assesses whether responses contain content related to violence, harm to others, or dangerous activities.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Violence Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate for violent content:")]
public class ViolenceEvaluatorLesson(IChatClient chatClient, ILogger<ViolenceEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        ViolenceEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}

