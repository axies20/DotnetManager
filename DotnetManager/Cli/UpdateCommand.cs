using System.CommandLine;

namespace DotnetManager.Cli;

public class UpdateCommand
{
    public Command Create()
    {
        var command = new Command("update", "Update all tracked .NET SDKs");
        return command;
    }
}
