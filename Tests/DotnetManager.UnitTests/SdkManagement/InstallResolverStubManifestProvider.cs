using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;

namespace DotnetManager.UnitTests.SdkManagement;

internal sealed class InstallResolverStubManifestProvider(
    SdkReleaseIndex index,
    IReadOnlyDictionary<Uri, SdkReleaseManifest> manifests) : ISdkManifestProviderService
{
    public Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(index);
    }

    public Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(manifests[manifestUri]);
    }
}
