namespace DotnetManager.Installation.Abstractions.Archives;

internal interface IArchiveExtractorService
{
    Task ExtractAsync(string archivePath, string destinationPath, CancellationToken cancellationToken);
}