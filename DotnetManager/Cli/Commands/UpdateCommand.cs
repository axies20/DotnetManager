using System.CommandLine;

namespace DotnetManager.Cli.Commands;

public class UpdateCommand
{
    public Command Create()
    {
        var command = new Command(
            "update",
            "Check every tracked .NET SDK channel for a newer eligible release and update its managed installation while preserving pinned versions.");
        return command;
    }
}
