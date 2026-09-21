namespace DotnetManager.SdkManagement.Abstractions.Archives;

public interface IArchiveExtractor
{
    Task ExtractAsync(string archivePath, string destinationPath, CancellationToken cancellationToken = default);
}