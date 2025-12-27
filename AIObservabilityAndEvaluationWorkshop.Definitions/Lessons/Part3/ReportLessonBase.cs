using System.Reflection;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

public abstract class ReportLessonBase(IChatClient chatClient, IConfiguration configuration) : LessonBase
{
    protected static async Task<IEnumerable<ScenarioRunResult>> GetLatestResultsAsync(ReportingConfiguration reportConfig, int count = 5)
    {
        List<ScenarioRunResult> results = [];
        await foreach (var name in reportConfig.ResultStore.GetLatestExecutionNamesAsync(count: count))
        {
            await foreach (var result in reportConfig.ResultStore.ReadResultsAsync(name))
            {
                results.Add(result);
            }
        }
        return results;
    }

    protected async Task GetResponseAndEvaluateAsync(string scenarioName, ReportingConfiguration reportConfig, ChatMessage[] messages, IEnumerable<EvaluationContext>? additionalContext = null)
    {
        ChatResponse response = await chatClient.GetResponseAsync(messages);

        await EvaluateAsync(scenarioName, reportConfig, messages, response, additionalContext);
    }

    protected static async Task EvaluateAsync(string scenarioName, ReportingConfiguration reportConfig, ChatMessage[] messages, ChatResponse response, IEnumerable<EvaluationContext>? additionalContext = null, IEnumerable<string>? tags = null)
    {
        ScenarioRun run = await reportConfig.CreateScenarioRunAsync(scenarioName: scenarioName, iterationName: GetDiskFriendlyDateString(), additionalTags: tags);
        await run.EvaluateAsync(messages, response, additionalContext);
        await run.DisposeAsync(); // ensure it gets flushed / fully written
    }
    
    protected string GetReportFileName()
    {
        LessonAttribute lessonAttribute = GetLessonAttribute();
        return $"{lessonAttribute.Part}_{lessonAttribute.Order}_Report.html";
    }

    protected ChatConfiguration GetChatConfiguration() => new(chatClient);
    
    private static string GetDiskFriendlyDateString() => DateTime.Now.ToString("yyyyMMdd_HHmmss");

    protected string GetReportPath(string filename)
    {
        // Get an asbolute path if this is a relative reference.
        string path = configuration["ReportsPath"] ?? "Reports";
        if (!Path.IsPathRooted(path))
        {
            path = Path.GetFullPath(path, Environment.CurrentDirectory);
        }
        path = Path.Combine(path, filename);
        return path;
    }

    protected static string GetReportMarkdown(string path)
    {
        Uri uri = new Uri(path);
        return $"""
                ### Evaluation and Report Generation Complete

                **Report location:**
                [{uri.AbsoluteUri}]({uri.AbsoluteUri})
                """;
    }
}