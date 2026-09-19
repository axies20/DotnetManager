using System.CommandLine;

namespace DotnetManager.Cli.Commands;

public class InstallCommand
{
    public Command Create()
    {
        var command = new Command(
            "install",
            "Download and install a .NET SDK selected by channel, support policy, or exact version into the configured installation directory.");
        return command;
    }
}