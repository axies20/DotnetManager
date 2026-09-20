using System.CommandLine;
using DotnetManager.Cli.Abstractions;

namespace DotnetManager.Cli.Commands;

public class InstallCommand : ICommand
{

    public Command Initialize()
    {
        var command = new Command(
            "install",
            "Download and install a .NET SDK selected by channel, support policy, or exact version into the configured installation directory.");
        return command;
    }
}