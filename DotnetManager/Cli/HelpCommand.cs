using System.CommandLine;

namespace DotnetManager.Cli;

public class HelpCommand
{
    public Command Create()
    {
        var command = new Command("help", "Show help and usage information");
        return command;
    }
}
