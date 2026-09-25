using DotnetManager.Installation.Abstractions.Downloads;
using DotnetManager.Installation.Models.Downloads;

namespace DotnetManager.UnitTests.SdkManagement;

internal sealed class OrchestratorStubDownloader(string directory) : IDotnetDownloaderService
{
    public List<string> Downloaded { get; } = [];

    public Task<DotnetDownload> DownloadAsync(DotnetDownloadSource downloadSource,
        CancellationToken cancellationToken)
    {
        Downloaded.Add(downloadSource.FileName);
        return Task.FromResult(new DotnetDownload(Path.Combine(directory, downloadSource.FileName)));
    }
}
