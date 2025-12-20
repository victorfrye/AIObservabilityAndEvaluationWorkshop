using AIObservabilityAndEvaluationWorkshop.Definitions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace AIObservabilityAndEvaluationWorkshop.ConsoleRunner;

public class DisplayCommand(ILogger<DisplayCommand> logger)
{
    private readonly ActivitySource _activitySource = new(typeof(DisplayCommand).FullName!);
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };


    public Task ExecuteAsync(string message)
    {
        // Create a named activity for better trace visibility
        using Activity? activity = _activitySource.StartActivity("DisplayCommand.Execute");
        activity?.SetTag("message", message);
        activity?.SetTag("command.name", "display");

        logger.LogInformation("Displaying message: {Message}", message);

        // Create result object
        ConsoleResult result = new ConsoleResult
            { Success = true, Input = message, Output = message, ErrorMessage = null };

        try
        {
            // Serialize and output the result in a standardized format for AppHost to capture
            // Use single-line JSON for console output (regex capture works better with single-line)
            string json = JsonSerializer.Serialize(result);
            Console.WriteLine($"CONSOLE_RESULT: {json}");
            // Log indented version for readability
            logger.LogInformation("Console result output: {Json}", JsonSerializer.Serialize(result, _jsonSerializerOptions));
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to serialize result");

            // Output error result instead
            ConsoleResult errorResult = new ConsoleResult
                { Success = false, Input = message, Output = null, ErrorMessage = ex.Message };

            try
            {
                // Use single-line JSON for console output
                string errorJson = JsonSerializer.Serialize(errorResult);
                Console.WriteLine($"CONSOLE_RESULT: {errorJson}");
                // Log indented version for readability
                logger.LogError("Console error result output: {Json}", JsonSerializer.Serialize(errorResult, _jsonSerializerOptions));
            }
            catch (Exception innerEx)
            {
                logger.LogError(innerEx, "Failed to serialize error result");
            }
            
            return Task.CompletedTask;
        }
    }
}
