using System.Diagnostics;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Formats.Html;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Reporting;

public class DiskReportStorageStrategy : IReportStorageStrategy
{
    private readonly ActivitySource _activitySource;
    private readonly ILogger<DiskReportStorageStrategy> _logger;
    private readonly IConfiguration _configuration;

    public DiskReportStorageStrategy(ILogger<DiskReportStorageStrategy> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _activitySource = new ActivitySource(GetType().FullName!);
    }

    public Task<ReportingConfiguration> CreateConfigurationAsync(IEnumerable<IEvaluator> evaluators)
    {
        using Activity? activity = _activitySource.StartActivity(nameof(CreateConfigurationAsync));
        string projectRoot = Directory.GetCurrentDirectory();
        string storagePath = _configuration["EvaluationResultsPath"] ?? Path.Combine(projectRoot, "EvaluationResults");
        activity?.SetTag("storage.path", storagePath);

        if (!Directory.Exists(storagePath))
        {
            _logger.LogInformation("Creating storage directory: {StoragePath}", storagePath);
            activity?.AddEvent(new ActivityEvent("Creating storage directory"));
            Directory.CreateDirectory(storagePath);
        }

        DiskBasedResultStore store = new(storagePath);
        return Task.FromResult(new ReportingConfiguration(evaluators, store));
    }

    public async Task<string> WriteReportAsync(ReportingConfiguration reportingConfig, ScenarioRunResult runResult, string filename)
    {
        using Activity? activity = _activitySource.StartActivity(nameof(WriteReportAsync), ActivityKind.Producer);

        string projectRoot = Directory.GetCurrentDirectory();
        string reportDir = _configuration["ReportsPath"] ?? Path.Combine(projectRoot, "Reports");        
        activity?.SetTag("report.path", reportDir);

        if (!Directory.Exists(reportDir))
        {
            _logger.LogInformation("Creating report directory: {ReportPath}", reportDir);
            activity?.AddEvent(new ActivityEvent("Creating report directory"));
            Directory.CreateDirectory(reportDir);
        }

        string reportPath = Path.Combine(reportDir, filename);
        activity?.SetTag("report.path", reportPath);
        HtmlReportWriter writer = new(reportPath);
        
        try 
        {
            _logger.LogInformation("Writing HTML report to {ReportPath}", Path.GetFullPath(reportPath));
            await writer.WriteReportAsync([runResult]);
            activity?.AddEvent(new ActivityEvent("Report written", tags: new ActivityTagsCollection { { "report.path", reportPath } }));
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Failed to write HTML report to {ReportPath}: {ExMessage} ({Name})", reportPath, ex.Message, ex.GetType().Name);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }

        return new FileInfo(reportPath).FullName.Replace('\\', '/');
    }
}
