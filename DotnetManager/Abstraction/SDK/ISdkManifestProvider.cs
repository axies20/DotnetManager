using DotnetManager.Models.Sdk.SdkManifest.ReleaseIndex;
using DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;

namespace DotnetManager.Abstraction.SDK;

public interface ISdkManifestProvider
{
    Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken = default);

    Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri, CancellationToken cancellationToken = default);
}
