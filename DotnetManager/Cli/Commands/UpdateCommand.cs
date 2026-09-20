using System.CommandLine;
using DotnetManager.Cli.Abstractions;

namespace DotnetManager.Cli.Commands;

public class UpdateCommand : ICommand
{
    public Command Create()
    {
        var command = new Command(
            "update",
            "Check every tracked .NET SDK channel for a newer eligible release and update its managed installation while preserving pinned versions.");
        return command;
    }

    public Command Initialize()
    {
        throw new NotImplementedException();
    }
}