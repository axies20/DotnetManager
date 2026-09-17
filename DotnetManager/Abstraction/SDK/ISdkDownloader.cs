using DotnetManager.Models.Sdk;
using DotnetManager.Models.Sdk.SdkDownloader;

namespace DotnetManager.Abstraction.SDK;

public interface ISdkDownloader
{
    Task<SdkDownload> DownloadAsync(SdkArtifact artifact, CancellationToken cancellationToken = default);
}