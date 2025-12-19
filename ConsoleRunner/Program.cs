// The console app receives the user input as a command-line argument from the AppHost.
// The AppHost uses IInteractionService to prompt the user, then passes the result here.
using TimeWarp.Nuru;

NuruApp app = new NuruAppBuilder()
  .AddRoute("display {message}", (string message) => Console.WriteLine(message))
  .Build();

return await app.RunAsync(args);