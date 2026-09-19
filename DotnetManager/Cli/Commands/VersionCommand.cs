using System.CommandLine;

namespace DotnetManager.Cli.Commands;

public class VersionCommand
{
    public Command Create()
    {
        var command = new Command(
            "version",
            "Print the installed DotnetManager application version for diagnostics, automation, and compatibility checks.");
        return command;
    }
}
