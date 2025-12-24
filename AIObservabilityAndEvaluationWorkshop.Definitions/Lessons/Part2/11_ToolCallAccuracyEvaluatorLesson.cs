#pragma warning disable AIEVAL001 // Experimental evaluator
using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 11, "Tool Call Accuracy Evaluator", needsInput: true,
    informationalScreenTitle: "Tool Call Accuracy Evaluator",
    informationalScreenMessage: "This lesson demonstrates the Tool Call Accuracy Evaluator, which assesses whether the AI correctly calls tools/functions with appropriate parameters and uses the results accurately.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Tool Call Accuracy Evaluator - Message Input",
    inputPromptMessage: "Enter a message that might require tool calls to evaluate for accuracy:")]
public class ToolCallAccuracyEvaluatorLesson(IChatClient chatClient, ILogger<ToolCallAccuracyEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        ToolCallAccuracyEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}

