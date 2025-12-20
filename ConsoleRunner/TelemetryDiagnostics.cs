using Microsoft.Extensions.Logging;

namespace AIObservabilityAndEvaluationWorkshop.ConsoleRunner;

/// <summary>
/// Logs OpenTelemetry environment variables on startup for diagnostic purposes.
/// </summary>
public static class TelemetryDiagnostics
{
    /// <summary>
    /// Logs OTEL environment variables on startup.
    /// </summary>
    public static void LogOtelEnvironmentVariables(ILogger logger)
    {
        try
        {
            // Log OTEL environment variables
            string? otelServiceName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME");
            string? otelResourceAttributes = Environment.GetEnvironmentVariable("OTEL_RESOURCE_ATTRIBUTES");
            string? otelExporterOtlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
            
            logger.LogInformation("OTEL_SERVICE_NAME: {Value}", otelServiceName ?? "(NOT SET)");
            logger.LogInformation("OTEL_RESOURCE_ATTRIBUTES: {Value}", otelResourceAttributes ?? "(NOT SET)");
            logger.LogInformation("OTEL_EXPORTER_OTLP_ENDPOINT: {Value}", otelExporterOtlpEndpoint ?? "(NOT SET)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log OTEL environment variables: {ex.Message}");
        }
    }
}

