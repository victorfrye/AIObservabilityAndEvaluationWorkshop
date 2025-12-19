// The console app receives the user input as a command-line argument from the AppHost.
// The AppHost uses IInteractionService to prompt the user, then passes the result here.
public partial class Program
{
    public static int Main(string[] args)
    {
        var message = "Hello, World!"; // Default value

        // Parse command-line arguments to get the message
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--message" && i + 1 < args.Length)
            {
                message = args[i + 1];
                break;
            }
        }

        // Log the message to the console
        Console.WriteLine(message);

        return 0;
    }
}