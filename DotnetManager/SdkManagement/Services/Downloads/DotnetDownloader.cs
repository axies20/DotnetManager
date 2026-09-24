using System.Security.Cryptography;
using DotnetManager.SdkManagement.Abstractions.Downloads;
using DotnetManager.SdkManagement.Models.Downloads;

namespace DotnetManager.SdkManagement.Services.Downloads;

public class DotnetDownloader(HttpClient client) : IDotnetDownloader
{
    public async Task<DotnetDownload> DownloadAsync(DotnetDownloadSource downloadSource,
        CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(Path.GetTempPath(), downloadSource.FileName);

        byte[] hashBytes;

        await using (var source = await client.GetStreamAsync(downloadSource.Uri, cancellationToken))
        await using (var destination = File.Create(filePath))
        {
            await source.CopyToAsync(destination, cancellationToken);
            await destination.FlushAsync(cancellationToken);
            destination.Position = 0;
            hashBytes = await SHA512.HashDataAsync(destination, cancellationToken);
        }

        var hash = Convert.ToHexString(hashBytes);

        if (string.Equals(hash, downloadSource.Hash, StringComparison.OrdinalIgnoreCase))
        {
            return new DotnetDownload(filePath);
        }

        File.Delete(filePath);
        throw new InvalidDataException($"Hash mismatch for {downloadSource.FileName}.");

    }
}