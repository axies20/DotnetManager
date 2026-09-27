using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Installation.Abstractions.Removal;

namespace DotnetManager.Cli.Commands.Remove;

public partial class RemoveCommand : ICommand
{
    private readonly IDotnetRemovalService _removalService;

    public RemoveCommand(IDotnetRemovalService removalService)
    {
        _removalService = removalService;
    }

    public Command Initialize()
    {
        var command = new Command("remove",
            "Stop tracking a .NET SDK channel or exact pinned version and remove its managed installation when it is no longer required.");
        command.Options.Add(_runtime);
        command.Options.Add(_sdk);
        command.Options.Add(_host);
        command.Options.Add(_asp);
        command.Arguments.Add(_version);
        command.SetAction(Execute);
        return command;
    }
}