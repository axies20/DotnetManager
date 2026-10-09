using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;
using DotnetManager.Installation.Abstractions.Archives;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Installation.Services.Archives;

internal sealed class ArchiveExtractor(ILogger<ArchiveExtractor> logger) : IArchiveExtractorService
{
    public async Task ExtractAsync(string archivePath, string destinationPath, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Extracting {ArchivePath} to {DestinationPath}",
            archivePath, destinationPath);
        Directory.CreateDirectory(destinationPath);

        if (archivePath.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
        {
            await ExtractTarGzAsync(archivePath, destinationPath, cancellationToken);
            LogCompleted(archivePath, stopwatch.Elapsed);
            return;
        }

        logger.LogWarning("Archive format is not supported for {ArchivePath}", archivePath);
        throw new NotSupportedException(
            $"Archive format is not supported: {archivePath}");
    }

    private static async Task ExtractTarGzAsync(string archivePath,
        string destinationPath,
        CancellationToken cancellationToken)
    {
        await using var fileStream = File.OpenRead(archivePath);
        await using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);

        await TarFile.ExtractToDirectoryAsync(gzipStream, destinationPath,
            true, cancellationToken);
    }

    private void LogCompleted(string archivePath, TimeSpan elapsed)
    {
        logger.LogInformation("Extracted {ArchivePath} in {ElapsedSeconds:F1} seconds",
            archivePath, elapsed.TotalSeconds);
    }
}