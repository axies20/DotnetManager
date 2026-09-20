using System.CommandLine;
using DotnetManager.Cli.Abstractions;

namespace DotnetManager.Cli.Commands.Available;

public class AvailableCommand : ICommand
{
    public Command Create()
    {
        var command = new Command("available",
            "Browse .NET SDK versions available for installation from the configured release metadata source, including releases for a selected channel and support policy.");
        return command;
    }

    public Command Initialize()
    {
        throw new NotImplementedException();
    }
}