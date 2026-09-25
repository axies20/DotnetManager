using DotnetManager.SdkManagement.Models.Downloads;

namespace DotnetManager.SdkManagement.Abstractions.Downloads;

public interface IDotnetDownloaderService
{
    Task<DotnetDownload> DownloadAsync(DotnetDownloadSource downloadSource,
        CancellationToken cancellationToken);
}
