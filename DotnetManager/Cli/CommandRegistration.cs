using System.CommandLine;
using DotnetManager.Cli.Abstractions;

namespace DotnetManager.Cli;

public sealed class CommandRegistration(IEnumerable<ICommand> commands)
{
    public void AddSubCommands(RootCommand rootCommand)
    {
        foreach (var command in commands)
        {
            rootCommand.Subcommands.Add(command.Initialize());
        }
    }
}