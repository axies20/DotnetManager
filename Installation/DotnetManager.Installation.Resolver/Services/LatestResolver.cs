using DotnetManager.Core.Models;
using DotnetManager.Installation.Resolver.Abstraction;
using DotnetManager.Installation.Resolver.Exceptions;
using DotnetManager.Installation.Resolver.Models.Results;
using DotnetManager.Installation.Resolver.Models.Targets;
using DotnetManager.Platform.Abstractions;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;

namespace DotnetManager.Installation.Resolver.Services;

internal sealed class LatestResolver(ISdkManifestProviderService provider, IHostPlatformService platform)
    : InstallResolverBase(platform), IInstallResolver<LatestSelector>
{

    public async Task<ResolvedInstallation> ResolveAsync(LatestSelector latest,
        IEnumerable<DotnetComponent> requestComponents,
        string? requestRid,
        CancellationToken cancellationToken)
    {
        var index = await provider.GetReleaseIndexAsync(cancellationToken);
        IEnumerable<SdkChannel> channels = index.Releases;

        if (latest.ReleaseType is {} releaseType)
            channels = channels.Where(x => x.ReleaseTypes == releaseType);

        if (latest.SupportPhase is {} supportPhase)
            channels = channels.Where(x => x.SupportPhase == supportPhase);

        var sdkChannel = channels.MaxBy(x => x.ChannelVersion);

        if (sdkChannel is null)
        {
            throw new InstallChannelNotFoundException(latest.ReleaseType, latest.SupportPhase);
        }

        var release = await provider.GetReleasesAsync(sdkChannel.ReleasesUri, cancellationToken);

        var latestRelease = release.Releases.MaxBy(x => x.ReleaseVersion) ??
                            throw InstallReleaseNotFoundException.ForChannel(release.ChannelVersion);

        return ResolveLatestReleaseAsync(release, requestComponents, latestRelease, requestRid);
    }


}