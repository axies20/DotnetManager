using DotnetManager.Models.Sdk.SdkManifest;

namespace DotnetManager.Abstraction.SDK;

public interface ISdkManifestProvider
{
    Task<SdkReleaseIndex> GetReleaseIndexAsync(CancellationToken cancellationToken = default);

    Task<SdkReleaseManifest> GetReleasesAsync(Uri manifestUri, CancellationToken cancellationToken = default);
}