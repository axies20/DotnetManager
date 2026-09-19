using System.CommandLine;
using DotnetManager.Cli.Commands;

namespace DotnetManager.Cli;

public sealed class CommandRegistration(
    AvailableCommand availableCommand,
    HelpCommand helpCommand,
    InstallCommand installCommand,
    ListCommand listCommand,
    RemoveCommand removeCommand,
    UpdateCommand updateCommand,
    VersionCommand versionCommand)
{
    public void AddSubCommands(RootCommand rootCommand)
    {
        rootCommand.Subcommands.Add(availableCommand.Create());
        rootCommand.Subcommands.Add(helpCommand.Create());
        rootCommand.Subcommands.Add(installCommand.Create());
        rootCommand.Subcommands.Add(listCommand.Initialize());
        rootCommand.Subcommands.Add(removeCommand.Create());
        rootCommand.Subcommands.Add(updateCommand.Create());
        rootCommand.Subcommands.Add(versionCommand.Create());
    }
}