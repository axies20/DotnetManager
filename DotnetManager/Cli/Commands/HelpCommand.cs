using System.CommandLine;

namespace DotnetManager.Cli.Commands;

public class HelpCommand
{
    public Command Create()
    {
        var command = new Command(
            "help",
            "Display detailed usage information for DotnetManager and its commands, including the available arguments, options, and examples.");
        return command;
    }
}
