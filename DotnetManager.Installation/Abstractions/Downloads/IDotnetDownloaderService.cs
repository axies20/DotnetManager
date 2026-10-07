using DotnetManager.Installation.Models.Downloads;

namespace DotnetManager.Installation.Abstractions.Downloads;

internal interface IDotnetDownloaderService
{
    Task<DotnetDownload> DownloadAsync(DotnetDownloadSource downloadSource, CancellationToken cancellationToken);
}