
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
[Lesson(3, 2, "Multiple Evaluators", needsInput: true,
    informationalScreenTitle: "Multiple Evaluators",
    informationalScreenMessage: "This lesson uses multiple evaluators in the same scenario run, including evaluators requiring context.",
    inputPromptTitle: "What conference and room did this session take place?",
    inputPromptMessage: "Enter a sentence answering the sample prompt")]
public class MultipleEvaluatorsLesson(
    IChatClient chatClient,
    IConfiguration configuration,
    ILogger<ReportGenerationLesson> logger,
    IEvaluationResultStore resultStore) : ReportLessonBase(chatClient, configuration)
{
    protected override async Task<string> RunAsync(string message)
    {
        IEvaluator rtcEval = new RelevanceTruthAndCompletenessEvaluator();
        IEvaluator equivalenceEval = new EquivalenceEvaluator();
        IEvaluator groundednessEval = new GroundednessEvaluator(); // Note: you could use GroundednessPro and UngroundedAttributes here if you have Content Safety connected
        
        ReportingConfiguration reportConfig = new([equivalenceEval, groundednessEval, rtcEval], 
            resultStore, 
            executionName: nameof(MultipleEvaluatorsLesson),
            chatConfiguration: GetChatConfiguration(), 
            tags: ["CodeMash"]);

        string groundTruth = "This session took place in Indigo Bay at CodeMash 2026";
        EvaluationContext[] context =
        [
            new EquivalenceEvaluatorContext(groundTruth),
            new GroundednessEvaluatorContext(groundTruth)
            // Note: RTC does not require additional context
        ];
        
        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a helpful assistant providing information. Please be curteous and professional at all times."),
            new(ChatRole.User, "What conference and room did this session take place?")
        ];
        ChatResponse response = new(new ChatMessage(ChatRole.Assistant, message));
        
        await EvaluateAsync("Groundedness and Equivalence Check for Conference and Room", reportConfig, messages, response, additionalContext: context);

        IEnumerable<ScenarioRunResult> results = await GetLatestResultsAsync(reportConfig);
        
        string filename = GetReportFileName();
        logger.LogDebug("Using report filename {Filename}", filename);
        
        string path = GetReportPath(filename);
        logger.LogDebug("Using report location {Path}", path);

        HtmlReportWriter writer = new(path);
        await writer.WriteReportAsync(results);
        
        return GetReportMarkdown(path);
    }
}