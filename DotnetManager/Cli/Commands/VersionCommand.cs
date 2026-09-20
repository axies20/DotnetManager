using System.CommandLine;
using DotnetManager.Cli.Abstractions;

namespace DotnetManager.Cli.Commands;

public class VersionCommand : ICommand
{
    public Command Create()
    {
        var command = new Command(
            "version",
            "Print the installed DotnetManager application version for diagnostics, automation, and compatibility checks.");
        return command;
    }

    public Command Initialize()
    {
        throw new NotImplementedException();
    }
}