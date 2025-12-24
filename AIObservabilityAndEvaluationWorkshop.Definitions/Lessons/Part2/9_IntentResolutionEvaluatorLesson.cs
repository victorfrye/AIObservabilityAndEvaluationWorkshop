#pragma warning disable AIEVAL001 // Experimental evaluator
using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 9, "Intent Resolution Evaluator", needsInput: true,
    informationalScreenTitle: "Intent Resolution Evaluator",
    informationalScreenMessage: "This lesson demonstrates the Intent Resolution Evaluator, which assesses how well the AI understands and correctly addresses the user's intent behind their query.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Intent Resolution Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate for intent resolution:")]
public class IntentResolutionEvaluatorLesson(IChatClient chatClient, ILogger<IntentResolutionEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        IntentResolutionEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}

