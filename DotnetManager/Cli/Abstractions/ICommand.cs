using System.CommandLine;

namespace DotnetManager.Cli.Abstractions;

internal interface ICommand
{
    Command Initialize();
}