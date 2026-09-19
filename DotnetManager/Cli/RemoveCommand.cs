using System.CommandLine;

namespace DotnetManager.Cli;

public class RemoveCommand
{
    public Command Create()
    {
        var command = new Command("remove", "Remove a tracked .NET SDK channel or pinned version");
        return command;
    }
}
