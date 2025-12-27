using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AIObservabilityAndEvaluationWorkshop.ConsoleRunner;

public static class ReportingExtensions
{

    public static HostApplicationBuilder AddReportStorage(this HostApplicationBuilder builder)
    {
        if (builder.Configuration["ReportStorageType"]?.ToLowerInvariant() == "azure")
        {
            //builder.Services.AddScoped<IEvaluationResultStore, AzureReportStorageStrategy>();
            throw new NotImplementedException("Azure storage coming soon");
        }
        else
        {
            builder.Services.AddScoped<IEvaluationResultStore>(_ =>
            {
                string path = builder.Configuration["EvaluationResultsPath"] ?? throw new InvalidOperationException("Ensure EvaluationResultsPath is configured");
        
                // Get an asbolute path if this is a relative reference.
                if (!Path.IsPathRooted(path))
                {
                    path = Path.GetFullPath( path, Environment.CurrentDirectory);
                }
        
                return new DiskBasedResultStore(path);
            });
        }

        return builder;
    }
}