using System.Formats.Tar;
using System.IO.Compression;
using DotnetManager.Installation.Abstractions.Archives;

namespace DotnetManager.Installation.Services.Archives;

public class ArchiveExtractor : IArchiveExtractorService
{
    public async Task ExtractAsync(string archivePath, string destinationPath, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(destinationPath);

        if (archivePath.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
        {
            await ExtractTarGzAsync(archivePath, destinationPath, cancellationToken);

            return;
        }

        if (archivePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            await ZipFile.ExtractToDirectoryAsync(archivePath, destinationPath,
                true, cancellationToken);
            return;
        }

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
}
