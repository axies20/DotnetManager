using DotnetManager.SdkManagement.Models.Downloads;

namespace DotnetManager.SdkManagement.Abstractions;

public interface ISdkDownloader
{
    Task<SdkDownload> DownloadAsync(SdkDownloadSource downloadSource, CancellationToken cancellationToken = default);
}