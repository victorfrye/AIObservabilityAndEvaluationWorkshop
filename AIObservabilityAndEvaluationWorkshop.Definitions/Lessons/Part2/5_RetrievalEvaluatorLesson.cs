using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 5, "Retrieval Evaluator", needsInput: true,
    informationalScreenTitle: "Retrieval Evaluator",
    informationalScreenMessage: "This lesson demonstrates the Retrieval Evaluator, which assesses how well the AI retrieves and uses relevant information from context or knowledge bases to answer the query.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Retrieval Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate for retrieval quality:")]
public class RetrievalEvaluatorLesson(IChatClient chatClient, ILogger<RetrievalEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        RetrievalEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}

