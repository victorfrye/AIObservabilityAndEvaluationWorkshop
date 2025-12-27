using Azure.Identity;
using Azure.Storage.Files.DataLake;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AIObservabilityAndEvaluationWorkshop.ConsoleRunner;

public static class ReportingExtensions
{

    public static HostApplicationBuilder AddReportStorage(this HostApplicationBuilder builder)
    {
        if (builder.Configuration["ReportStorageType"]?.ToLowerInvariant() == "azure")
        {
            bool useIdentity = builder.Configuration.GetValue("AIUseIdentity", false);
            if (!useIdentity)
            {
                throw new InvalidOperationException("Azure Identity authentication is required when using Azure Report Storage");
            }
                
            builder.Services.AddScoped<IEvaluationResultStore>(_ =>
            {
                DataLakeDirectoryClient client = new(
                    new Uri(
                        baseUri: new Uri(builder.Configuration["AzureStorageDataLakeEndpoint"]!),
                        relativeUri: builder.Configuration["AzureStorageContainer"]),
                    credential: new AzureCliCredential());
                
                return new AzureStorageResultStore(client);
            });
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