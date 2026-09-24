using System.Runtime.InteropServices;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using DotnetManager.SdkManagement.Abstractions.Resolver;
using DotnetManager.SdkManagement.Models.Downloads;
using DotnetManager.SdkManagement.Models.Installation.Requests;
using DotnetManager.SdkManagement.Models.Installation.Targets;
using NuGet.Versioning;

namespace DotnetManager.SdkManagement.Services.Resolver;

public class DotnetInstallResolver : IDotnetInstallResolver
{
    private readonly ISdkManifestProvider _provider;

    public DotnetInstallResolver(ISdkManifestProvider provider)
    {
        _provider = provider;
    }

    public async Task<IReadOnlyCollection<DotnetDownloadSource>> ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken)
    {
        var index = await _provider.GetReleaseIndexAsync(cancellationToken);
        IEnumerable<SdkChannel> channels = index.Releases;
        return request.Target switch
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
    }


    private static IReadOnlyCollection<DotnetDownloadSource> CreateDownloadSources(
        SdkRelease release,
        InstallOptions options)
    {
        var rid = string.IsNullOrWhiteSpace(options.RuntimeIdentifier)
            ? RuntimeInformation.RuntimeIdentifier
            : options.RuntimeIdentifier;

        return options.Components
            .Select(component => ResolveFile(release, component, rid))
            .Select(file => new DotnetDownloadSource(file.Url, file.FileName, file.Hash))
            .ToList();
    }


    private static ReleaseFile ResolveFile(SdkRelease release, InstallComponent component, string rid)
    {
        return component switch
        {
            InstallComponent.Sdk => ResolveLatestSdkFile(release, rid),
            InstallComponent.Runtime => ResolveArtifact(
                release.Runtime, rid, "runtime", "dotnet-runtime"),
            InstallComponent.AspNetRuntime => ResolveArtifact(
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
            throw new InvalidOperationException(
                $"Release {release.ReleaseVersion} contains no SDKs.");

        return ResolveArtifact(sdk, rid, "SDK", "dotnet-sdk");
    }

    private static ReleaseFile ResolveArtifact(
        DotnetVersion version,
        string rid,
        string componentName,
        string archiveName)
    {
        var tarGzName = $"{archiveName}-{rid}.tar.gz";
        var zipName = $"{archiveName}-{rid}.zip";

        return version.Artifacts.FirstOrDefault(x =>
                   x.Rid == rid &&
                   (x.FileName.Equals(tarGzName, StringComparison.OrdinalIgnoreCase) ||
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
            x.ChannelVersion.Major == channelVersion.Major &&
            x.ChannelVersion.Minor == channelVersion.Minor);

        if (channel is null)
        {
            throw new InvalidOperationException($".NET channel {version.Major}.{version.Minor} was not found.");
        }


        return CreateDownloadsAsync(channel.ReleasesUri, options,
            releases =>
                releases.FirstOrDefault(x => x.ReleaseVersion == version),
            cancellationToken);
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

        var latestChannel = channels.MaxBy(
            x => x.ChannelVersion,
            VersionComparer.VersionRelease);

        if (latestChannel is null)
            throw new InvalidOperationException(
                "No matching .NET channel was found.");

        return CreateDownloadsAsync(latestChannel.ReleasesUri, options,
            releases =>
            {
                if (latestSelector.SecurityOnly)
                    releases = releases.Where(x => x.Security);

                return releases.MaxBy(x => x.ReleaseVersion, VersionComparer.VersionRelease);
            },
            cancellationToken);
    }


    private async Task<IReadOnlyCollection<DotnetDownloadSource>> CreateDownloadsAsync(
        Uri releaseUri,
        InstallOptions options,
        Func<IEnumerable<SdkRelease>, SdkRelease?> selectRelease,
        CancellationToken cancellationToken)
    {
        var manifest = await _provider.GetReleasesAsync(releaseUri, cancellationToken);

        var release = selectRelease(manifest.Releases) ??
                      throw new InvalidOperationException("No matching .NET release was found.");

        return CreateDownloadSources(release, options);
    }
}
