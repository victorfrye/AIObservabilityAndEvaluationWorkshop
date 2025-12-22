using AIObservabilityAndEvaluationWorkshop.Definitions;
using JetBrains.Annotations;
using Microsoft.Extensions.AI;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
public class OllamaLesson(IChatClient chatClient) : LessonBase
{
    public override string DisplayName => "Ollama Chat";

    public override bool NeedsInput => true;

    protected override async Task<string> RunAsync(string message)
    {
        ChatResponse response = await chatClient.GetResponseAsync(message);
        return response.ToString();
    }
}
