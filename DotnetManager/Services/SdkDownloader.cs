using DotnetManager.Abstraction.SDK;
using DotnetManager.Models.Sdk.SdkDownloader;

namespace DotnetManager.Services;

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