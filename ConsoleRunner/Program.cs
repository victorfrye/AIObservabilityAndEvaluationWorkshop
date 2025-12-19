// The console app receives the user input as a command-line argument from the AppHost.
// The AppHost uses IInteractionService to prompt the user, then passes the result here.
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using TimeWarp.Nuru;

var builder = Host.CreateApplicationBuilder(args);

// Add Aspire service defaults for telemetry
builder.AddServiceDefaults();

// Configure Nuru telemetry
var activitySource = new ActivitySource("ConsoleRunner");

// Define output file path
var outputFilePath = Path.Combine(AppContext.BaseDirectory, "console_output.txt");

// Add Nuru app with telemetry integration
NuruApp app = new NuruAppBuilder()
  .AddRoute("display {message}", (string message) =>
  {
      using var activity = activitySource.StartActivity("DisplayMessage");
      activity?.SetTag("message", message);

      // Write to console (for local debugging)
      Console.WriteLine(message);

      // Write to shared output file for AppHost to read
      try
      {
          var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
          var outputLine = $"[{timestamp}] {message}";
          File.AppendAllText(outputFilePath, outputLine + Environment.NewLine);
      }
      catch (Exception ex)
      {
          Console.WriteLine($"Error writing to output file: {ex.Message}");
      }
  })
  .Build();

// Build the host and run the Nuru app
var host = builder.Build();
await host.StartAsync();

// Run the Nuru app - this will capture telemetry through the host
return await app.RunAsync(args);