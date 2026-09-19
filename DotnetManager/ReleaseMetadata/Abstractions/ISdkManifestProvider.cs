using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;

namespace DotnetManager.ReleaseMetadata.Abstractions;

public interface ISdkManifestProvider
{
    Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken = default);

    Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri, CancellationToken cancellationToken = default);
}
