using System.CommandLine;

namespace DotnetManager.Cli.Commands;

public class ListCommand
{
    public Command Create()
    {
        var command = new Command(
            "list",
            "Show the configured SDK sources together with the .NET SDKs discovered in the managed installation and other known .NET locations.");
        return command;
    }
}
