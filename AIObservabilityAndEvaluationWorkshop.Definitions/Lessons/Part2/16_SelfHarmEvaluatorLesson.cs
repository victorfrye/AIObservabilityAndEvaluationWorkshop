using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Safety;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 16, "Self Harm Evaluator", needsInput: true,
    informationalScreenTitle: "Self Harm Evaluator",
    informationalScreenMessage: "This lesson demonstrates the Self Harm Evaluator, which assesses whether responses contain content related to self-harm, suicide, or other self-destructive behaviors.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Self Harm Evaluator - Message Input",
    inputPromptMessage: "Enter a message to evaluate for self-harm content:")]
public class SelfHarmEvaluatorLesson(IChatClient chatClient, ILogger<SelfHarmEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        SelfHarmEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}

