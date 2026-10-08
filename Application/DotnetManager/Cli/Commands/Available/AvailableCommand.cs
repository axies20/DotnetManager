using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Cli.Commands.Available.Abstraction;

namespace DotnetManager.Cli.Commands.Available;

internal sealed partial class AvailableCommand(ISdkReleaseService sdkReleaseService) : ICommand
{
    public Command Initialize()
    {
        var command = new Command("available",
            "Browse .NET SDK versions available for installation from the configured release metadata source, including releases for a selected channel and support policy.");

        command.Arguments.Add(_channelArgument);
        command.SetAction(Execute);
        return command;
    }
}