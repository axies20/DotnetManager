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
        var latest = FindLatestVersion(manifest, selector) ?? throw new InstallReleaseNotFoundException(version);


        return ResolveRelease(manifest, components, latest, rid);

    }


    private static SdkRelease? FindLatestVersion(SdkReleaseManifest manifest, VersionSelector selector)
    {
        var range = CreateRange(selector);

        return manifest.Releases
            .Where(x => range.Satisfies(x.ReleaseVersion))
            .MaxBy(x => x.ReleaseVersion);
    }

    private static VersionRange CreateRange(VersionSelector selector)
    {
        var version = selector.Version;

        return selector.Precision switch
        {
            1 => new VersionRange(
                new NuGetVersion(version.Major, 0, 0, "0"),
                true,
                new NuGetVersion(version.Major + 1, 0, 0, "0"),
                false),

            2 => new VersionRange(
                new NuGetVersion(version.Major, version.Minor, 0, "0"),
                true,
                new NuGetVersion(version.Major, version.Minor + 1, 0, "0"),
                false),

            _ => new VersionRange(version, true, version, true)
        };
    }
}