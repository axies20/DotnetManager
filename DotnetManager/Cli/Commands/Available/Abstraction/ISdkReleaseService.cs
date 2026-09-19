using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using NuGet.Versioning;

namespace DotnetManager.Cli.Commands.Available.Abstraction;

public interface ISdkReleaseService
{
    Task<IReadOnlyCollection<SdkChannel>> GetChannelsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SdkRelease>> GetReleasesAsync(NuGetVersion channelVersion,
        CancellationToken cancellationToken = default);
}