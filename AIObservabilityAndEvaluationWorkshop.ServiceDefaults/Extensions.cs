using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

// Adds common Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        var otlpExporterConfigured = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        
        var openTelemetryBuilder = builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation(aspNetCoreTracing =>
                        // Exclude health check requests from tracing
                        aspNetCoreTracing.Filter = context =>
                            !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                            && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                    )
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
                
                // For console apps, also register ActivitySources using wildcard pattern to catch all sources
                // This ensures any ActivitySource created in the application namespace will be captured
                // The pattern matches any ActivitySource name starting with the application name
                string wildcardPattern = $"{builder.Environment.ApplicationName}*";
                tracing.AddSource(wildcardPattern);
            });

        // Chain the OTLP exporter to the same builder to ensure proper configuration
        // This is critical - calling AddOpenTelemetry() again creates a separate builder
        if (otlpExporterConfigured)
        {
            // Try to use Aspire's UseOtlpExporter() extension method if available
            // If that doesn't work, fall back to manual OTLP exporter configuration
            try
            {
                // UseOtlpExporter() is an Aspire extension method that configures the OTLP exporter
                // It reads OTEL_EXPORTER_OTLP_ENDPOINT from configuration automatically
                openTelemetryBuilder.UseOtlpExporter();
            }
            catch
            {
                // Fallback: Manually configure OTLP exporter if UseOtlpExporter() is not available
                // This ensures telemetry export works even if the Aspire extension method fails
                var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] 
                    ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                
                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                {
                    openTelemetryBuilder
                        .WithTracing(tracing => tracing.AddOtlpExporter())
                        .WithMetrics(metrics => metrics.AddOtlpExporter())
                        .WithLogging(logging => logging.AddOtlpExporter());
                }
            }
        }

        return builder;
    }


    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks(HealthEndpointPath);

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }

}