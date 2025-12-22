using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 1, "Simple evaluator in action", needsInput: true)]
public class SimpleEvaluatorLesson(IChatClient chatClient) : LessonBase
{
    protected override async Task<string> RunAsync(string message)
    {
        // Evaluate the fluency of the user's input
        // Using the same chatClient for evaluation
        FluencyEvaluator evaluator = new();
        
        EvaluationResult evaluationResult = await evaluator.EvaluateAsync(
            message,
            message,
            chatConfiguration: new ChatConfiguration(chatClient));

        evaluationResult.Metrics.TryGetValue(FluencyEvaluator.FluencyMetricName, out EvaluationMetric? fluencyMetric);
        
        double grade = (fluencyMetric as NumericMetric)?.Value ?? 0;
        string reasoning = fluencyMetric?.Interpretation?.Reason ?? "No reasoning provided.";

        // Build the markdown response
        return $"""
               ### Input
               {message}

               ### Fluency Evaluation
               **Grade:** {grade} / 5
               
               **Reasoning:**
               {reasoning}
               """;
    }
}
