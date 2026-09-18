using DotnetManager.Models.Sdk.SdkDownloader;

namespace DotnetManager.Abstraction.SDK;

public interface ISdkDownloader
{
    Task<SdkDownload> DownloadAsync(SdkDownloadSource downloadSource, CancellationToken cancellationToken = default);
}