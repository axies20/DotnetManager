namespace DotnetManager.SdkManagement.Abstractions.Archives;

public interface IArchiveExtractorService
{
    Task ExtractAsync(string archivePath, string destinationPath, CancellationToken cancellationToken);
}
