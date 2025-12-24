using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Safety;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 12, "Groundedness Pro Evaluator", needsInput: true,
    informationalScreenTitle: "Groundedness Pro Evaluator",
    informationalScreenMessage: "This lesson demonstrates the Groundedness Pro Evaluator, an advanced version that provides more detailed analysis of whether responses are grounded in factual information with enhanced verification capabilities.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Groundedness Pro Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate for groundedness (Pro):")]
public class GroundednessProEvaluatorLesson(IChatClient chatClient, ILogger<GroundednessProEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        GroundednessProEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}

