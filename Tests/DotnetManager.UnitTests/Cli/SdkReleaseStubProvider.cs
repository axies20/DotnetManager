using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;

namespace DotnetManager.UnitTests.Cli;

internal sealed class SdkReleaseStubProvider(SdkChannel channel, SdkReleaseManifest manifest)
    : ISdkManifestProviderService
{
    public Uri? RequestedManifestUri { get; private set; }

    public Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new SdkReleaseIndex([channel]));
    }

    public Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri,
        CancellationToken cancellationToken)
    {
        RequestedManifestUri = manifestUri;
        return Task.FromResult(manifest);
    }
}
