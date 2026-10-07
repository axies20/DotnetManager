using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Cli.Commands.Update;

internal sealed class UpdateCommand(ILogger<UpdateCommand> logger) : ICommand
{
    public Command Initialize()
    {
        var command = new Command(
            "update",
            "Check every tracked .NET SDK channel for a newer eligible release and update its managed installation while preserving pinned versions.");
        command.SetAction(_ =>
        {
            logger.LogWarning("The update command is not implemented yet");
            return 1;
        });
        return command;
    }
}
