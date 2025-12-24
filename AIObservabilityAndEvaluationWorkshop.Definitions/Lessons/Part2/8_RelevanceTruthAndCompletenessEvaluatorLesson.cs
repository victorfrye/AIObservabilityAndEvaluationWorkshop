#pragma warning disable AIEVAL001 // Experimental evaluator
using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 8, "Relevance Truth And Completeness Evaluator", needsInput: true,
    informationalScreenTitle: "Relevance Truth And Completeness Evaluator",
    informationalScreenMessage: "This lesson demonstrates a composite evaluator that assesses relevance, truthfulness, and completeness together. It provides a comprehensive evaluation of response quality across multiple dimensions.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Relevance Truth And Completeness Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate for relevance, truth, and completeness:")]
public class RelevanceTruthAndCompletenessEvaluatorLesson(IChatClient chatClient, ILogger<RelevanceTruthAndCompletenessEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        RelevanceTruthAndCompletenessEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}

