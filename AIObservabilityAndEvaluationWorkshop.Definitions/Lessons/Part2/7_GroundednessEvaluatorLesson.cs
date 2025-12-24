using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 7, "Groundedness Evaluator", needsInput: true,
    informationalScreenTitle: "Groundedness Evaluator",
    informationalScreenMessage: "This lesson demonstrates the Groundedness Evaluator, which assesses whether the AI's response is grounded in factual information and can be verified against source material. It helps detect hallucinations.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Groundedness Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate for groundedness:")]
public class GroundednessEvaluatorLesson(IChatClient chatClient, ILogger<GroundednessEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        GroundednessEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}

