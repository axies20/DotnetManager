using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Removal.Abstractions;

namespace DotnetManager.Cli.Commands.Remove;

internal sealed partial class RemoveCommand : ICommand
{
    private readonly IDotnetRemovalService _removalService;

    public RemoveCommand(IDotnetRemovalService removalService)
    {
        _removalService = removalService;
    }

    public Command Initialize()
    {
        var command = new Command("remove",
            "Remove discovered .NET component directories matching a major, major.minor, or exact version.");
        command.Options.Add(_runtime);
        command.Options.Add(_sdk);
        command.Options.Add(_host);
        command.Options.Add(_asp);
        command.Arguments.Add(_version);
        command.SetAction(Execute);
        return command;
    }
}