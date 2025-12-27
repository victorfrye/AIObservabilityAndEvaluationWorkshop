
using JetBrains.Annotations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Formats.Html;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

#pragma warning disable AIEVAL001

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Lessons;

[UsedImplicitly]
[Lesson(3, 3, "Multiple Scenarios", needsInput: false,
    informationalScreenTitle: "Multiple Scenarios",
    informationalScreenMessage: "This lesson uses multiple scenario runs in the same report on a single AI agent. AI responses are simulated for purposes of speed and consistency in demonstration.")]
public class MultipleScenariossLesson(
    IChatClient chatClient,
    IConfiguration configuration,
    ILogger<ReportGenerationLesson> logger,
    IEvaluationResultStore resultStore) : ReportLessonBase(chatClient, configuration)
{
    protected override async Task<string> RunAsync(string message)
    {
        ReportingConfiguration reportConfig = new([new RelevanceTruthAndCompletenessEvaluator()], 
            resultStore,
            executionName: nameof(MultipleScenariossLesson),
            chatConfiguration: GetChatConfiguration(), 
            tags: ["CodeMash"]);

        string groundTruth = "This session took place in Indigo Bay at CodeMash 2026";
        EvaluationContext[] context =
        [
            new EquivalenceEvaluatorContext(groundTruth),
            new GroundednessEvaluatorContext(groundTruth)
            // Note: RTC does not require additional context
        ];

        string systemPrompt = "You are a helpful assistant providing information. Please be curteous and professional at all times.";
        
        await RunNorthAmericanCountriesScenarioAsync(systemPrompt, reportConfig);
        await RunCommonProgrammingLanguagesAsync(systemPrompt, reportConfig);
        
        IEnumerable<ScenarioRunResult> results = await GetLatestResultsAsync(reportConfig);
        
        string filename = GetReportFileName();
        logger.LogDebug("Using report filename {Filename}", filename);
        
        string path = GetReportPath(filename);
        logger.LogDebug("Using report location {Path}", path);

        HtmlReportWriter writer = new(path);
        await writer.WriteReportAsync(results);
        
        return GetReportMarkdown(path);
    }

    private static async Task RunNorthAmericanCountriesScenarioAsync(string systemPrompt, ReportingConfiguration reportConfig)
    {
        ChatMessage[] messages =
        [
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, "What countries are in North America?")
        ];
        ChatResponse response = new(new ChatMessage(ChatRole.Assistant, "North America consists of Canada, Mexico, the New California Republic, and the United States of America"));
        await EvaluateAsync("North American Countries", reportConfig, messages, response, tags: ["Geography"]);
    }
    
    private static async Task RunCommonProgrammingLanguagesAsync(string systemPrompt, ReportingConfiguration reportConfig)
    {
        ChatMessage[] messages =
        [
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, "What are some common programming languages?")
        ];
        ChatResponse response = new(new ChatMessage(ChatRole.Assistant, "Programming languages commonly used include C#, Java, COBOL, Lisp, JavaScript (a variant of Java), bf, Swift, Python, Ruby, and VBScript."));
        await EvaluateAsync("Common Programming Languages", reportConfig, messages, response, tags: ["Programming"]);
    }
}