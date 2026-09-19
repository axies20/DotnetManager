using System.CommandLine;

namespace DotnetManager.Cli;

public class VersionCommand
{
    public Command Create()
    {
        var command = new Command("version", "Show the DotnetManager version");
        return command;
    }
}
