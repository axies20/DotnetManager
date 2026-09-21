using DotnetManager.SdkManagement.Models.Downloads;

namespace DotnetManager.SdkManagement.Abstractions.Downloads;

public interface IDotnetDownloader
{
    Task<DotnetDownload> DownloadAsync(DotnetDownloadSource downloadSource,
        CancellationToken cancellationToken = default);
}