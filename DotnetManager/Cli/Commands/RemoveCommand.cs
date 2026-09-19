using System.CommandLine;

namespace DotnetManager.Cli.Commands;

public class RemoveCommand
{
    public Command Create()
    {
        var command = new Command(
            "remove",
            "Stop tracking a .NET SDK channel or exact pinned version and remove its managed installation when it is no longer required.");
        return command;
    }
}