using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 2, "Completeness Evaluator", needsInput: true)]
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
