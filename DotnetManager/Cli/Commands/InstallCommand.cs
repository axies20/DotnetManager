using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Exception.Installation;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Models;
using DotnetManager.Installation.Models.Installation.Requests;
using DotnetManager.Installation.Models.Installation.Targets;
using DotnetManager.ReleaseMetadata.Models;
using NuGet.Versioning;

namespace DotnetManager.Cli.Commands;

public class InstallCommand : ICommand
{
    private readonly IDotnetInstallOrchestratorService _dotnetInstallOrchestrator;

    private readonly Command _latest = new("latest",
        "Install the latest version of the specified release type and support phase");

    private readonly Option<ReleaseTypes?> _releaseType = new("--release-type", "-rt")
    {
        Description = "Install the latest version of the specified release type"
    };

    private readonly Option<SupportPhases?> _supportPhase = new("--support-phase", "-sp")
    {
        Description = "Install the latest version of the specified support phase"
    };

    private readonly Option<bool> _includeNonSecurity = new("--include-non-security", "-ins")
    {
        Description = "Allow installation of non-security releases"
    };

    private readonly Option<bool> _runtime = new("--runtime", "-rt")
    {
        Description = "Install the .NET runtime instead of the SDK",
        Recursive = true
    };

    private readonly Option<bool> _aspnet = new("--aspnet", "-asp")
    {
        Description = "Install the ASP.NET Core runtime instead of the SDK",
        Recursive = true
    };

    private readonly Option<string?> _rid = new("--rid")
    {
        Description = "Install the .NET runtime with the specified runtime identifier",
        Recursive = true
    };


    private readonly Argument<string> _version = new("version")
    {
        Description = "Exact .NET release version to install"
    };


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

    private static string GetErrorMessage(DotnetInstallException exception)
    {
        return exception switch
        {
            InstallChannelNotFoundException { ChannelVersion: not null } error =>
                $"The .NET channel '{error.ChannelVersion}' was not found.",

            InstallChannelNotFoundException error =>
                $"No .NET channel matches release type '{error.ReleaseType?.ToString() ?? "Any"}' " +
                $"and support phase '{error.SupportPhase?.ToString() ?? "Any"}'.",

            InstallReleaseNotFoundException { Version: not null } error =>
                $"The .NET release '{error.Version}' was not found.",

            InstallReleaseNotFoundException { SecurityOnly: true } error =>
                $"No security release was found in .NET channel '{error.ChannelVersion}'. " +
                "Use --include-non-security to allow other releases.",

            InstallReleaseNotFoundException error =>
                $"No release was found in .NET channel '{error.ChannelVersion}'.",

            InstallSdkNotFoundException error =>
                $"The .NET release '{error.ReleaseVersion}' does not contain an SDK.",

            InstallArtifactNotFoundException error =>
                $"{error.ComponentName} {error.Version} is not available for RID " +
                $"'{error.RuntimeIdentifier}'.",

            DownloadHashMismatchException error =>
                $"SHA-512 verification failed for '{error.FileName}'. The downloaded file is corrupted.",

            InstalledDotnetExecutableNotFoundException error =>
                $"The extracted files in '{error.InstallRoot}' do not contain the dotnet executable.",

            _ => "The .NET installation failed."
        };
    }

    private Task<int> ExecuteLatest(ParseResult result, CancellationToken cancellationToken)
    {
        var rid = result.GetValue(_rid);
        var includeNonSecurity = result.GetValue(_includeNonSecurity);
        var release = result.GetValue(_releaseType);
        var phase = result.GetValue(_supportPhase);
        var installRequest = new InstallRequest
        {
            Options = new InstallOptions
            {
                Components = GetInstallComponents(result),
                RuntimeIdentifier = rid
            },
            Target = new LatestSelector(release, phase, !includeNonSecurity)
        };

        return ExecuteAsync(installRequest, cancellationToken);
    }

    private Task<int> ExecuteInstall(ParseResult result, CancellationToken cancellationToken)
    {
        var rid = result.GetValue(_rid);
        var value = result.GetValue(_version);

        if (!NuGetVersion.TryParse(value, out var version))
        {
            Console.Error.WriteLine($"Invalid version: {value}");
            return Task.FromResult(1);
        }

        var installRequest = new InstallRequest
        {
            Options = new InstallOptions
            {
                Components = GetInstallComponents(result),
                RuntimeIdentifier = rid
            },
            Target = new VersionSelector(version)
        };
        return ExecuteAsync(installRequest, cancellationToken);
    }

    private async Task<int> ExecuteAsync(InstallRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _dotnetInstallOrchestrator.InstallAsync(request, cancellationToken);
            return 0;
        }
        catch (DotnetInstallException exception)
        {
            await Console.Error.WriteLineAsync(GetErrorMessage(exception));
            return 1;
        }
        catch (HttpRequestException exception)
        {
            await Console.Error.WriteLineAsync($"Unable to download .NET files: {exception.Message}");
            return 1;
        }
    }

    private IReadOnlyCollection<DotnetComponent> GetInstallComponents(ParseResult result)
    {
        List<DotnetComponent> components = [];

        if (result.GetValue(_runtime))
            components.Add(DotnetComponent.Runtime);

        if (result.GetValue(_aspnet))
            components.Add(DotnetComponent.AspNetRuntime);

        if (components.Count == 0)
            components.Add(DotnetComponent.Sdk);

        return components;
    }
}