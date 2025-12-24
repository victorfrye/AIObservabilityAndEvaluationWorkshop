using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 3, "Coherence Evaluator", needsInput: true)]
public class CoherenceEvaluatorLesson(IChatClient chatClient, ILogger<CoherenceEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        CoherenceEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync( message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}
