using System.CommandLine;

namespace DotnetManager.Cli.Commands;

public class AvailableCommand
{
    public Command Create()
    {
        var command = new Command(
            "available",
            "Browse .NET SDK versions available for installation from the configured release metadata source, including releases for a selected channel and support policy.");
        return command;
    }
}