using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 2, "Completeness Evaluator", needsInput: true,
    informationalScreenTitle: "Completeness Evaluator",
    informationalScreenMessage: "This lesson demonstrates the Completeness Evaluator, which assesses whether the AI's response fully addresses the user's question or request. It checks if all aspects of the query are covered.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Completeness Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate for completeness:")]
public class CompletenessEvaluatorLesson(IChatClient chatClient, ILogger<CompletenessEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        CompletenessEvaluator evaluator = new();
        
        // TODO: Needs CompletenessEvaluatorContext 
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync( message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}
