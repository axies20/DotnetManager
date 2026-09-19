using DotnetManager.SdkManagement.Abstractions;
using DotnetManager.SdkManagement.Models.Downloads;

namespace DotnetManager.SdkManagement.Services;

public class SdkDownloader(HttpClient client) : ISdkDownloader
{
    public async Task<SdkDownload> DownloadAsync(SdkDownloadSource downloadSource,
        CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(Path.GetTempPath(), downloadSource.FileName);

        await using var source = await client.GetStreamAsync(downloadSource.Uri, cancellationToken);
        await using var destination = File.Create(filePath);
        await source.CopyToAsync(destination, cancellationToken);

        return new SdkDownload(filePath);
    }
}