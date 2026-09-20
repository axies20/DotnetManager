using System.CommandLine;

namespace DotnetManager.Cli.Abstractions;

public interface ICommand
{
    Command Initialize();
}