using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Installation.Abstractions.Installation;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Cli.Commands.Install;

internal sealed partial class InstallCommand : ICommand
{
    private readonly IDotnetInstallOrchestratorService _dotnetInstallOrchestrator;
    private readonly ILogger<InstallCommand> _logger;

    public InstallCommand(IDotnetInstallOrchestratorService dotnetInstallOrchestrator,
        ILogger<InstallCommand> logger)
    {
        _dotnetInstallOrchestrator = dotnetInstallOrchestrator;
        _logger = logger;
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
        command.Options.Add(_releaseVersion);

        _latest.Options.Add(_releaseType);
        _latest.Options.Add(_supportPhase);

        //command.SetAction(ExecuteInstall);
        //_latest.SetAction(ExecuteLatest);
        return command;
    }
}