// The console app receives the user input as a command-line argument from the AppHost.
// The AppHost uses IInteractionService to prompt the user, then passes the result here.
using Microsoft.Extensions.CommandLineUtils;

public partial class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandLineApplication();
        app.Name = "ConsoleRunner";
        app.Description = "Console application that displays a message";

        var messageOption = app.Option("-m|--message <MESSAGE>", "The message to display", CommandOptionType.SingleValue);

        app.OnExecute(() =>
        {
            var message = messageOption.Value() ?? "Hello, World!";
            Console.WriteLine(message);
            return 0;
        });

        return app.Execute(args);
    }
}