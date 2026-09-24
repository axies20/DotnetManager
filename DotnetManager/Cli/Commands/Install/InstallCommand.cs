using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.ReleaseMetadata.Models;
using DotnetManager.SdkManagement.Abstractions.Installation;
using DotnetManager.SdkManagement.Models.Installation.Requests;
using DotnetManager.SdkManagement.Models.Installation.Targets;
using NuGet.Versioning;

namespace DotnetManager.Cli.Commands.Install;

public class InstallCommand : ICommand
{
    private readonly IDotnetInstallOrchestrator _dotnetInstallOrchestrator;

    private readonly Command _latest = new("latest",
        "Install the latest version of the specified release type and support phase");

    private readonly Option<ReleaseTypes> _releaseType = new("--release-type",
        "Install the latest version of the specified release type");

    private readonly Option<SupportPhases> _supportPhase = new("--support-phase",
        "Install the latest version of the specified support phase");

    private readonly Option<bool> _includeNonSecurity = new("--include-non-security",
        "Allow installation of non-security releases");

    private readonly Option<bool> _runtime = new("--runtime",
        "Install the .NET runtime instead of the SDK")
    {
        Recursive = true
    };

    private readonly Option<bool> _aspnet = new("--aspnet",
        "Install the ASP.NET Core runtime instead of the SDK")
    {
        Recursive = true
    };

    private readonly Option<string?> _rid = new("--rid",
        "Install the .NET runtime with the specified runtime identifier")
    {
        Recursive = true
    };


    private readonly Argument<string> _version = new("version")
    {
        Description = "SDK channel or exact version to install"
    };


    public InstallCommand(IDotnetInstallOrchestrator dotnetInstallOrchestrator)
    {
        _dotnetInstallOrchestrator = dotnetInstallOrchestrator;
    }


    public Command Initialize()
    {
        var command = new Command("install",
            "Download and install a .NET SDK selected by channel, support policy, or exact version into the configured installation directory.");
        command.Arguments.Add(_version);

        command.Subcommands.Add(_latest);

        command.Options.Add(_releaseType);
        command.Options.Add(_supportPhase);
        _latest.Options.Add(_includeNonSecurity);

        command.SetAction(ExecuteInstall);
        _latest.SetAction(ExecuteLatest);
        return command;
    }

    private async Task ExecuteLatest(ParseResult result, CancellationToken cancellationToken)
    {
        var rid = result.GetValue(_rid);
        var includeNonSecurity = result.GetValue(_includeNonSecurity);
        var release = result.GetValue(_releaseType);
        var phase = result.GetValue(_supportPhase);
        var installRequest = new InstallRequest()
        {
            Options = new InstallOptions()
            {
                Components = GetInstallComponents(result),
                RuntimeIdentifier = rid
            },
            Target = new LatestSelector(release, phase, !includeNonSecurity)
        };

        await _dotnetInstallOrchestrator.InstallAsync(installRequest, cancellationToken);
    }

    private Task ExecuteInstall(ParseResult result, CancellationToken cancellationToken)
    {
        var rid = result.GetValue(_rid);
        var value = result.GetValue(_version);

        if (!NuGetVersion.TryParse(value, out var version))
        {
            Console.Error.WriteLine($"Invalid version: {value}");
            return Task.CompletedTask;
        }

        var installRequest = new InstallRequest()
        {
            Options = new InstallOptions()
            {
                Components = GetInstallComponents(result),
                RuntimeIdentifier = rid
            },
            Target = new VersionSelector(version)
        };
        return _dotnetInstallOrchestrator.InstallAsync(installRequest, cancellationToken);
    }

    private IReadOnlyCollection<InstallComponent> GetInstallComponents(ParseResult result)
    {
        List<InstallComponent> components = [];

        if (result.GetValue(_runtime))
            components.Add(InstallComponent.Runtime);

        if (result.GetValue(_aspnet))
            components.Add(InstallComponent.AspNetRuntime);

        if (components.Count == 0)
            components.Add(InstallComponent.Sdk);

        return components;
    }

}