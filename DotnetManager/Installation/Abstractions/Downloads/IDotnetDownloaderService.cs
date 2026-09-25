using DotnetManager.Installation.Models.Downloads;

namespace DotnetManager.Installation.Abstractions.Downloads;

public interface IDotnetDownloaderService
{
    Task<DotnetDownload> DownloadAsync(DotnetDownloadSource downloadSource,
        CancellationToken cancellationToken);
}
