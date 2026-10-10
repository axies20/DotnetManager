using DotnetManager.Core.Models;
using DotnetManager.Installation.Resolver.Abstraction;
using DotnetManager.Installation.Resolver.Exceptions;
using DotnetManager.Installation.Resolver.Models.Results;
using DotnetManager.Installation.Resolver.Models.Targets;
using DotnetManager.Platform.Abstractions;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using NuGet.Versioning;

namespace DotnetManager.Installation.Resolver.Services;

public class VersionResolver : InstallResolverBase, IInstallResolver<VersionSelector>
{
    private readonly ISdkManifestProviderService _provider;

    public VersionResolver(ISdkManifestProviderService provider, IHostPlatformService platform) : base(platform)
    {
        _provider = provider;
    }

    public async Task<ResolvedInstallation> ResolveAsync(VersionSelector selector,
        IEnumerable<DotnetComponent> components,
        string? rid,
        CancellationToken cancellationToken)
    {
        var index = await _provider.GetReleaseIndexAsync(cancellationToken);
        IEnumerable<SdkChannel> channels = index.Releases;

        var version = selector.Version;
        var channel = channels.FirstOrDefault(x =>
            x.ChannelVersion.Major == version.Major &&
            x.ChannelVersion.Minor == version.Minor);

        if (channel is null)
            throw new InstallChannelNotFoundException(version);

        var manifest = await _provider.GetReleasesAsync(channel.ReleasesUri, cancellationToken);
        var latest = FindLatestVersion(manifest, version) ?? throw new InstallReleaseNotFoundException(version);


        return ResolveLatestReleaseAsync(manifest, components, latest, rid);

    }


    private SdkRelease? FindLatestVersion(SdkReleaseManifest manifest, NuGetVersion version)
    {
        if (version.Major != 0 && version is { Minor: 0, Patch: 0 })
        {
            return manifest.Releases.FirstOrDefault(x =>
                x.ReleaseVersion == manifest.LatestRelease);
        }

        if (version.Major != 0 && version.Minor != 0)
        {
            var range = new VersionRange(
                new NuGetVersion(version.Major, version.Minor, 0),
                includeMinVersion: true,
                new NuGetVersion(version.Major, version.Minor + 1, 0),
                includeMaxVersion: true);

            return manifest.Releases
                .Where(x => range.Satisfies(x.ReleaseVersion))
                .MaxBy(x => x.ReleaseVersion);
        }

        return manifest.Releases.FirstOrDefault(x => x.ReleaseVersion == version);
    }
}