using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Cli.Commands;
using DotnetManager.Cli.Commands.Available;
using DotnetManager.Cli.Commands.List;

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