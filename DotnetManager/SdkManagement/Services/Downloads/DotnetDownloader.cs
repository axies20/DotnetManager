using DotnetManager.SdkManagement.Abstractions.Downloads;
using DotnetManager.SdkManagement.Models.Downloads;

namespace DotnetManager.SdkManagement.Services.Downloads;

public class DotnetDownloader(HttpClient client) : IDotnetDownloader
{
    public async Task<DotnetDownload> DownloadAsync(DotnetDownloadSource downloadSource,
        CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(Path.GetTempPath(), downloadSource.FileName);

        await using var source = await client.GetStreamAsync(downloadSource.Uri, cancellationToken);
        await using var destination = File.Create(filePath);
        await source.CopyToAsync(destination, cancellationToken);

        return new DotnetDownload(filePath);
    }
}