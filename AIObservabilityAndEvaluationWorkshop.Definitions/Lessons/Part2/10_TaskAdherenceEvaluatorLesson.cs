#pragma warning disable AIEVAL001 // Experimental evaluator
using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 10, "Task Adherence Evaluator", needsInput: true,
    informationalScreenTitle: "Task Adherence Evaluator",
    informationalScreenMessage: "This lesson demonstrates the Task Adherence Evaluator, which assesses whether the AI's response adheres to the specific task or instruction given by the user.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "Task Adherence Evaluator - Message Input",
    inputPromptMessage: "Enter a message with a task or instruction to evaluate for adherence:")]
public class TaskAdherenceEvaluatorLesson(IChatClient chatClient, ILogger<TaskAdherenceEvaluatorLesson> logger) : EvaluatorLessonBase(logger)
{
    protected override async Task<EvaluationResult> EvaluateAsync(string message)
    {
        TaskAdherenceEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        return evaluationResult;
    }
}

