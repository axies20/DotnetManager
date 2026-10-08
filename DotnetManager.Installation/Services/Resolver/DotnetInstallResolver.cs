using System.Runtime.InteropServices;
using DotnetManager.Installation.Exceptions;
using DotnetManager.Installation.Abstractions.Resolver;
using DotnetManager.Core.Models;
using DotnetManager.Installation.Models.Downloads;
using DotnetManager.Installation.Models.Installation.Requests;
using DotnetManager.Installation.Models.Installation.Targets;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace DotnetManager.Installation.Services.Resolver;

internal sealed class DotnetInstallResolver : IDotnetInstallResolverService
{
    private readonly ISdkManifestProviderService _provider;
    private readonly ILogger<DotnetInstallResolver> _logger;

    public DotnetInstallResolver(ISdkManifestProviderService provider,
        ILogger<DotnetInstallResolver> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<DotnetDownloadSource>> ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resolving .NET artifacts for {TargetType}",
            request.Target.GetType().Name);
        var index = await _provider.GetReleaseIndexAsync(cancellationToken);
        IEnumerable<SdkChannel> channels = index.Releases;
        var sources = request.Target switch
        {
            LatestSelector latestSelector => await CreateLatestAsync(
                channels,
                latestSelector,
                request.Options,
                cancellationToken),
            VersionSelector versionSelector => await CreateVersionAsync(
                channels,
                versionSelector,
                request.Options,
                cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Target))
        };

        _logger.LogInformation("Resolved {ArtifactCount} artifact(s) for download",
            sources.Count);
        return sources;
    }


    private static IReadOnlyCollection<DotnetDownloadSource> CreateDownloadSources(
        SdkRelease release,
        InstallOptions options)
    {
        string rid;

        if (string.IsNullOrWhiteSpace(options.RuntimeIdentifier))
            rid = RuntimeInformation.RuntimeIdentifier;
        else
            rid = options.RuntimeIdentifier;

        return options.Components
            .Select(component => ResolveFile(release, component, rid))
            .Select(file => new DotnetDownloadSource(file.Url, file.FileName, file.Hash))
            .ToList();
    }


    private static ReleaseFile ResolveFile(SdkRelease release, DotnetComponent component, string rid)
    {
        return component switch
        {
            DotnetComponent.Sdk => ResolveLatestSdkFile(release, rid),
            DotnetComponent.Runtime => ResolveArtifact(
                release.Runtime, rid, "runtime", "dotnet-runtime"),
            DotnetComponent.AspNetRuntime => ResolveArtifact(
                release.AspNetCoreRuntime, rid, "ASP.NET Core runtime", "aspnetcore-runtime"),
            _ => throw new ArgumentOutOfRangeException(nameof(component), component,
                "Unsupported install component.")
        };
    }

    private static ReleaseFile ResolveLatestSdkFile(SdkRelease release, string rid)
    {
        var sdk = release.Sdks.MaxBy(x => x.Version,
            VersionComparer.VersionRelease);

        if (sdk is null)
            throw new InstallSdkNotFoundException(release.ReleaseVersion);

        return ResolveArtifact(sdk, rid, "SDK", "dotnet-sdk");
    }

    private static ReleaseFile ResolveArtifact(DotnetVersion version,
        string rid,
        string componentName,
        string archiveName)
    {
        var tarGzName = $"{archiveName}-{rid}.tar.gz";
        var zipName = $"{archiveName}-{rid}.zip";

        var first = version.Artifacts.Where(file => file.Rid == rid)
            .FirstOrDefault(file =>
            {
                if (file.FileName.Equals(tarGzName, StringComparison.OrdinalIgnoreCase))
                    return true;

                return file.FileName.Equals(zipName, StringComparison.OrdinalIgnoreCase);
            });

        return first ?? throw new InstallArtifactNotFoundException(componentName, version.Version, rid);
    }

    private Task<IReadOnlyCollection<DotnetDownloadSource>> CreateVersionAsync(
        IEnumerable<SdkChannel> channels,
        VersionSelector versionSelector,
        InstallOptions options,
        CancellationToken cancellationToken)
    {
        var version = versionSelector.Version;

        var channelVersion = new NuGetVersion(version.Major, version.Minor, 0);

        var channel = channels.FirstOrDefault(x =>
        {
            if (x.ChannelVersion.Major != channelVersion.Major)
                return false;

            return x.ChannelVersion.Minor == channelVersion.Minor;
        });

        if (channel is null)
            throw new InstallChannelNotFoundException(channelVersion);


        return CreateDownloadsAsync(channel.ReleasesUri, options,
            releases =>
            {
                var sdkRelease = releases.FirstOrDefault(x => x.ReleaseVersion == version);

                if (sdkRelease == null)
                {
                    throw new InstallReleaseNotFoundException(version);
                }

                return sdkRelease;
            }, cancellationToken);
    }


    private Task<IReadOnlyCollection<DotnetDownloadSource>> CreateLatestAsync(
        IEnumerable<SdkChannel> channels,
        LatestSelector latestSelector,
        InstallOptions options,
        CancellationToken cancellationToken)
    {
        if (latestSelector.SupportPhase is not null)
            channels = channels.Where(x => x.SupportPhase == latestSelector.SupportPhase);

        if (latestSelector.ReleaseType is not null)
            channels = channels.Where(x => x.ReleaseTypes == latestSelector.ReleaseType);

        var latestChannel = channels.MaxBy(x => x.ChannelVersion, VersionComparer.VersionRelease);

        if (latestChannel is null)
            throw new InstallChannelNotFoundException(
                latestSelector.ReleaseType,
                latestSelector.SupportPhase);

        return CreateDownloadsAsync(latestChannel.ReleasesUri, options,
            releases => releases.MaxBy(x => x.ReleaseVersion, VersionComparer.VersionRelease) ??
                        throw InstallReleaseNotFoundException.ForChannel(latestChannel.ChannelVersion),
            cancellationToken);
    }


    private async Task<IReadOnlyCollection<DotnetDownloadSource>> CreateDownloadsAsync(
        Uri releaseUri,
        InstallOptions options,
        Func<IEnumerable<SdkRelease>, SdkRelease> selectRelease,
        CancellationToken cancellationToken)
    {
        var manifest = await _provider.GetReleasesAsync(releaseUri, cancellationToken);

        var release = selectRelease(manifest.Releases);

        _logger.LogInformation("Selected .NET release {ReleaseVersion}", release.ReleaseVersion);
        _logger.LogDebug("Resolving artifacts for runtime identifier {RuntimeIdentifier}",
            string.IsNullOrWhiteSpace(options.RuntimeIdentifier)
                ? RuntimeInformation.RuntimeIdentifier
                : options.RuntimeIdentifier);


        return CreateDownloadSources(release, options);
    }
}
