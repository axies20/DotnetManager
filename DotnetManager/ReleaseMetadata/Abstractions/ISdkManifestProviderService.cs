using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;

namespace DotnetManager.ReleaseMetadata.Abstractions;

public interface ISdkManifestProviderService
{
    Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken);

    Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri, CancellationToken cancellationToken);
}
