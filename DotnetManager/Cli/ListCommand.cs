using System.CommandLine;

namespace DotnetManager.Cli;

public class ListCommand
{
    public Command Create()
    {
        var command = new Command("list", "List configured sources and installed .NET SDKs");
        return command;
    }
}
