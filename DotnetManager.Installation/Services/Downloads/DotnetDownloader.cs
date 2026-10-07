using System.Buffers;
using System.Diagnostics;
using System.Security.Cryptography;
using DotnetManager.Installation.Exceptions;
using DotnetManager.Installation.Abstractions.Downloads;
using DotnetManager.Installation.Models.Downloads;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Installation.Services.Downloads;

internal sealed class DotnetDownloader(HttpClient client, ILogger<DotnetDownloader> logger) : IDotnetDownloaderService
{
    private const int BufferSize = 81920;
    private static readonly TimeSpan ProgressInterval = TimeSpan.FromSeconds(5);

    public async Task<DotnetDownload> DownloadAsync(DotnetDownloadSource downloadSource,
        CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(Path.GetTempPath(), downloadSource.FileName);
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Downloading {FileName} from {Uri}",
            downloadSource.FileName, downloadSource.Uri);

        byte[] hashBytes;

        using (var response = await client.GetAsync(downloadSource.Uri,
                   HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var contentLength = response.Content.Headers.ContentLength;
            LogDownloadSize(downloadSource.FileName, contentLength);

            await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);

            await using (var destination = File.Create(filePath))
            {
                var downloadedBytes = await CopyWithProgressAsync(source, destination,
                    downloadSource.FileName, contentLength, cancellationToken);
                await destination.FlushAsync(cancellationToken);
                destination.Position = 0;
                logger.LogInformation("Downloaded {FileName}: {DownloadedBytes} bytes in {ElapsedSeconds:F1} seconds",
                    downloadSource.FileName, downloadedBytes, stopwatch.Elapsed.TotalSeconds);

                logger.LogInformation("Verifying SHA-512 checksum for {FileName}",
                    downloadSource.FileName);
                hashBytes = await SHA512.HashDataAsync(destination, cancellationToken);
            }
        }

        var hash = Convert.ToHexString(hashBytes);

        if (string.Equals(hash, downloadSource.Hash, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("\nSHA-512 verification succeeded for {FileName}",
                downloadSource.FileName);
            return new DotnetDownload(filePath);
        }

        logger.LogWarning("\nSHA-512 verification failed for {FileName}; deleting {FilePath}",
            downloadSource.FileName, filePath);
        File.Delete(filePath);
        throw new DownloadHashMismatchException(downloadSource.FileName, downloadSource.Hash, hash);
    }

    private void LogDownloadSize(string fileName, long? contentLength)
    {
        if (contentLength is > 0)
        {
            logger.LogInformation("Download size for {FileName}: {TotalBytes} bytes",
                fileName, contentLength.Value);
            return;
        }

        logger.LogInformation("Download size for {FileName} is unknown", fileName);
    }

    private async Task<long> CopyWithProgressAsync(Stream source,
        Stream destination,
        string fileName,
        long? contentLength,
        CancellationToken cancellationToken)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);

        try
        {
            long downloadedBytes = 0;
            var nextPercentage = 10;
            var progressStopwatch = Stopwatch.StartNew();

            while (true)
            {
                var read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length),
                    cancellationToken);

                if (read == 0)
                    return downloadedBytes;

                await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                downloadedBytes += read;

                if (contentLength is > 0)
                {
                    var percentage = (int)Math.Min(100,
                        downloadedBytes * 100 / contentLength.Value);

                    if (percentage < nextPercentage)
                        continue;

                    logger.LogInformation(
                        "Downloading {FileName}: {ProgressPercent}% ({DownloadedBytes}/{TotalBytes} bytes)",
                        fileName, percentage, downloadedBytes, contentLength.Value);
                    nextPercentage = percentage >= 100
                        ? int.MaxValue
                        : percentage / 10 * 10 + 10;
                    continue;
                }

                if (progressStopwatch.Elapsed < ProgressInterval)
                    continue;

                logger.LogInformation("Downloading {FileName}: {DownloadedBytes} bytes received",
                    fileName, downloadedBytes);
                progressStopwatch.Restart();
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
