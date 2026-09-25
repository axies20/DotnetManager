using DotnetManager.Installation.Abstractions.Archives;

namespace DotnetManager.UnitTests.SdkManagement;

internal sealed class OrchestratorRecordingExtractor : IArchiveExtractorService
{
    public List<(string Archive, string Destination)> Extractions { get; } = [];

    public Task ExtractAsync(string archivePath, string destinationPath,
        CancellationToken cancellationToken)
    {
        Extractions.Add((archivePath, destinationPath));
        return Task.CompletedTask;
    }
}
