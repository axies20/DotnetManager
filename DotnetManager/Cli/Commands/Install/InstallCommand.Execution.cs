using System.CommandLine;
using DotnetManager.Installation.Exceptions;
using DotnetManager.Core.Models;
using DotnetManager.Installation.Models.Installation.Requests;
using DotnetManager.Installation.Models.Installation.Targets;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace DotnetManager.Cli.Commands.Install;

internal sealed partial class InstallCommand
{
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
        var release = result.GetValue(_releaseType);
        var phase = result.GetValue(_supportPhase);
        var installRequest = new InstallRequest
        {
            Options = new InstallOptions
            {
                Components = GetInstallComponents(result),
                RuntimeIdentifier = rid
            },
            Target = new LatestSelector(release, phase)
        };

        return ExecuteAsync(installRequest, cancellationToken);
    }

    private Task<int> ExecuteInstall(ParseResult result, CancellationToken cancellationToken)
    {
        var rid = result.GetValue(_rid);
        var value = result.GetValue(_version);

        if (!NuGetVersion.TryParse(value, out var version))
        {
            _logger.LogError("Invalid .NET version: {Version}", value);
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
        PrintInstallRequest(request);

        try
        {
            await _dotnetInstallOrchestrator.InstallAsync(request, cancellationToken);
            return 0;
        }
        catch (DotnetInstallException exception)
        {
            _logger.LogError("Installation failed: {ErrorMessage}", GetErrorMessage(exception));
            return 1;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Unable to download .NET files");
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
