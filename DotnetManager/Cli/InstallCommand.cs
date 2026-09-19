using System.CommandLine;

namespace DotnetManager.Cli;

public class InstallCommand
{
    public Command Create()
    {
        var command = new Command("install", "Install a .NET SDK");
        return command;
    }
}