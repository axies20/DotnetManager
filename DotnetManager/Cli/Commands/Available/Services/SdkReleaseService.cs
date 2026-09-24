using DotnetManager.Cli.Commands.Available.Abstraction;
using DotnetManager.Exception;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;
using NuGet.Versioning;

namespace DotnetManager.Cli.Commands.Available.Services;

public class SdkReleaseService : ISdkReleaseService
{
    private readonly ISdkManifestProvider _manifestProvider;

    public SdkReleaseService(ISdkManifestProvider manifestProvider)
    {
        _manifestProvider = manifestProvider;
    }

    public async Task<IReadOnlyCollection<SdkChannel>> GetChannelsAsync(CancellationToken cancellationToken)
    {
        return (await _manifestProvider.GetReleaseIndexAsync(cancellationToken)).Releases;
    }

    public async Task<SdkReleaseManifest> GetReleasesAsync(NuGetVersion channelVersion,
        CancellationToken cancellationToken)
    {
        var index = await _manifestProvider.GetReleaseIndexAsync(cancellationToken);

        var channel = index.Releases.FirstOrDefault(r => r.ChannelVersion == channelVersion);

        if (channel is null)
        {
            throw new SdkChannelNotFoundException(channelVersion);
        }

        return await _manifestProvider.GetReleasesAsync(channel.ReleasesUri,
            cancellationToken);
    }
}