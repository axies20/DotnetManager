using DotnetManager.Abstraction.SDK;
using DotnetManager.Models.Sdk.SdkDownloader;

namespace DotnetManager.Services;

public class SdkDownloader(HttpClient client) : ISdkDownloader
{
    public async Task<SdkDownload> DownloadAsync(SdkArtifact artifact, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(Path.GetTempPath(), artifact.FileName);

        await using var source = await client.GetStreamAsync(artifact.DownloadUri, cancellationToken);
        await using var destination = File.Create(filePath);
        await source.CopyToAsync(destination, cancellationToken);

        return new SdkDownload(filePath, artifact);
    }
}