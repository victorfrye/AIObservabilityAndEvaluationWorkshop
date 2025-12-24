using JetBrains.Annotations;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(2, 28, "NLP Evaluators", needsInput: true,
    informationalScreenTitle: "NLP Evaluators",
    informationalScreenMessage: "This lesson demonstrates NLP (Natural Language Processing) evaluators such as BLEU, GLEU, and F1 that use statistical methods to evaluate text quality and similarity.",
    informationalScreenSupportsMarkdown: false,
    inputPromptTitle: "NLP Evaluators - Message Input",
    inputPromptMessage: "Enter a message to evaluate using NLP metrics:")]
public class NLPEvaluatorsLesson : LessonBase
{
    protected override Task<string> RunAsync(string message)
    {
        return Task.FromResult($"NLP Evaluators placeholder. Input: {message}");
    }
}

