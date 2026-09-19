using System.CommandLine;

namespace DotnetManager.Cli;

public class AvailableCommand
{
    public Command Create()
    {
        var command = new Command("available", "List available .NET SDKs for a channel");
        return command;
    }
}
