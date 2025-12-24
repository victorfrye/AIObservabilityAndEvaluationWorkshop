using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Formats.Html;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.Definitions.Reporting;

public class DiskReportStorageStrategy(ILogger<DiskReportStorageStrategy> logger, IConfiguration configuration) : IReportStorageStrategy
{
    public Task<ReportingConfiguration> CreateConfigurationAsync(IEnumerable<IEvaluator> evaluators)
    {
        string projectRoot = Directory.GetCurrentDirectory();
        string storagePath = configuration["EvaluationResultsPath"] ?? Path.Combine(projectRoot, "EvaluationResults");

        if (!Directory.Exists(storagePath))
        {
            logger.LogInformation("Creating storage directory: {StoragePath}", storagePath);
            Directory.CreateDirectory(storagePath);
        }

        DiskBasedResultStore store = new(storagePath);
        return Task.FromResult(new ReportingConfiguration(evaluators, store));
    }

    public async Task<string> WriteReportAsync(ReportingConfiguration reportingConfig, ScenarioRunResult runResult, string filename)
    {
        string projectRoot = Directory.GetCurrentDirectory();
        string reportDir = configuration["ReportsPath"] ?? Path.Combine(projectRoot, "Reports");        if (!Directory.Exists(reportDir))
        {
            logger.LogInformation("Creating report directory: {ReportPath}", reportDir);
            Directory.CreateDirectory(reportDir);
        }

        string reportPath = Path.Combine(reportDir, filename);
        HtmlReportWriter writer = new(reportPath);
        
        try 
        {
            logger.LogInformation("Writing HTML report to {ReportPath}", Path.GetFullPath(reportPath));
            await writer.WriteReportAsync([runResult]);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "Failed to write HTML report to {ReportPath}: {ExMessage} ({Name})", reportPath, ex.Message, ex.GetType().Name);
            throw;
        }

        return new FileInfo(reportPath).FullName.Replace('\\', '/');
    }
}
