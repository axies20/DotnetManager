using System.Runtime.InteropServices;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using DotnetManager.SdkManagement.Models.Downloads;
using DotnetManager.SdkManagement.Models.Installation;
using NuGet.Versioning;

namespace DotnetManager.SdkManagement.Services.Resolver;

public sealed class DotnetInstallPlanner
{
    private readonly ISdkManifestProvider _provider;

    public DotnetInstallPlanner(ISdkManifestProvider provider)
    {
        _provider = provider;
    }

    public Task<IReadOnlyCollection<DotnetDownloadSource>> CreateAsync(
        InstallRequest request,
        CancellationToken cancellationToken = default)
    {
        return request.Target switch
        {
            LatestSelector latestSelector => CreateLatestAsync(
                latestSelector,
                request.Components,
                request.Security,
                request.RuntimeIdentifier,
                cancellationToken),
            VersionSelector versionSelector => CreateVersionAsync(
                versionSelector,
                request.Components,
                request.Security,
                request.RuntimeIdentifier,
                cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Target))
        };
    }

    private async Task<IReadOnlyCollection<DotnetDownloadSource>> CreateLatestAsync(
        LatestSelector latestSelector,
        IEnumerable<InstallComponent> components,
        bool security,
        string? runtimeIdentifier,
        CancellationToken cancellationToken)
    {
        var index = await _provider.GetReleaseIndexAsync(cancellationToken);
        IEnumerable<SdkChannel> channels = index.Releases;

        if (latestSelector.SupportPhase is not null)
            channels = channels.Where(x => x.SupportPhase == latestSelector.SupportPhase);

        if (latestSelector.ReleaseType is not null)
            channels = channels.Where(x => x.ReleaseTypes == latestSelector.ReleaseType);

        var latestChannel = channels.MaxBy(
            x => x.ChannelVersion,
            VersionComparer.VersionRelease);

        if (latestChannel is null)
            throw new InvalidOperationException("No matching .NET channel was found.");

        var manifest = await _provider.GetReleasesAsync(
            latestChannel.ReleasesUri,
            cancellationToken);
        IEnumerable<SdkRelease> releases = manifest.Releases;

        if (security)
            releases = releases.Where(x => x.Security);

        var latestRelease = releases.MaxBy(
            x => x.ReleaseVersion,
            VersionComparer.VersionRelease);

        if (latestRelease is null)
            throw new InvalidOperationException("No matching .NET release was found.");

        var rid = string.IsNullOrWhiteSpace(runtimeIdentifier)
            ? RuntimeInformation.RuntimeIdentifier
            : runtimeIdentifier;

        return components
            .Select(component => ResolveFile(latestRelease, component, rid))
            .Select(file => new DotnetDownloadSource(file.Url, file.FileName))
            .ToList();
    }

    private Task<IReadOnlyCollection<DotnetDownloadSource>> CreateVersionAsync(
        VersionSelector versionSelector,
        IEnumerable<InstallComponent> components,
        bool security,
        string? runtimeIdentifier,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static ReleaseFile ResolveFile(SdkRelease release, InstallComponent component, string rid)
    {
        return component switch
        {
            InstallComponent.Sdk => ResolveLatestSdkFile(release, rid),
            InstallComponent.Runtime => ResolveArtifact(release.Runtime, rid, "runtime"),
            InstallComponent.AspNetRuntime => ResolveArtifact(
                release.AspNetCoreRuntime, rid, "ASP.NET Core runtime"),
            _ => throw new ArgumentOutOfRangeException(nameof(component), component,
                "Unsupported install component.")
        };
    }

    private static ReleaseFile ResolveLatestSdkFile(SdkRelease release, string rid)
    {
        var sdk = release.Sdks.MaxBy(
            x => x.Version,
            VersionComparer.VersionRelease);

        if (sdk is null)
            throw new InvalidOperationException(
                $"Release {release.ReleaseVersion} contains no SDKs.");

        return ResolveArtifact(sdk, rid, "SDK");
    }

    private static ReleaseFile ResolveArtifact(DotnetVersion version, string rid, string componentName)
    {
        return version.Artifacts.FirstOrDefault(x => x.Rid == rid) ??
               throw new PlatformNotSupportedException(
                   $"{componentName} {version.Version} is not available for RID '{rid}'.");
    }
}