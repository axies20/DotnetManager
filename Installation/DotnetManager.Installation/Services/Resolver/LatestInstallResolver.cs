using DotnetManager.Installation.Exceptions;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using NuGet.Versioning;

namespace DotnetManager.Installation.Services.Resolver;

internal sealed class LatestInstallResolver(ISdkManifestProviderService provider)
    : IInstallReleaseResolverService<LatestSelector>
{
    public async Task<SdkRelease> ResolveAsync(IEnumerable<SdkChannel> channels,
        LatestSelector selector,
        CancellationToken cancellationToken)
    {
        if (selector.SupportPhase is not null)
            channels = channels.Where(x => x.SupportPhase == selector.SupportPhase);

        if (selector.ReleaseType is not null)
            channels = channels.Where(x => x.ReleaseTypes == selector.ReleaseType);

        var channel = channels.MaxBy(x => x.ChannelVersion, VersionComparer.VersionRelease);

        if (channel is null)
            throw new InstallChannelNotFoundException(selector.ReleaseType, selector.SupportPhase);

        var manifest = await provider.GetReleasesAsync(channel.ReleasesUri, cancellationToken);

        return manifest.Releases.MaxBy(x => x.ReleaseVersion, VersionComparer.VersionRelease) ??
               throw InstallReleaseNotFoundException.ForChannel(channel.ChannelVersion);
    }
}