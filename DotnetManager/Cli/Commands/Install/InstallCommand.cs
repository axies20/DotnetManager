using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Installation.Abstractions.Installation;

namespace DotnetManager.Cli.Commands.Install;

public partial class InstallCommand : ICommand
{
    private readonly IDotnetInstallOrchestratorService _dotnetInstallOrchestrator;

    public InstallCommand(IDotnetInstallOrchestratorService dotnetInstallOrchestrator)
    {
        _dotnetInstallOrchestrator = dotnetInstallOrchestrator;
    }

    public Command Initialize()
    {
        var command = new Command("install",
            "Download and install .NET components selected by latest-release filters or an exact release version.");
        command.Arguments.Add(_version);

        command.Subcommands.Add(_latest);

        command.Options.Add(_runtime);
        command.Options.Add(_aspnet);
        command.Options.Add(_rid);
        _latest.Options.Add(_releaseType);
        _latest.Options.Add(_supportPhase);
        _latest.Options.Add(_includeNonSecurity);

        command.SetAction(ExecuteInstall);
        _latest.SetAction(ExecuteLatest);
        return command;
    }
}