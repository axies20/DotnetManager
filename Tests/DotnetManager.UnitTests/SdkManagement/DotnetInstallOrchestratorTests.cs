using DotnetManager.Installation.Abstractions.Archives;
using DotnetManager.Installation.Abstractions.Downloads;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Abstractions.InstallPaths;
using DotnetManager.Installation.Abstractions.Resolver;
using DotnetManager.Installation.Models;
using DotnetManager.Installation.Models.Downloads;
using DotnetManager.Installation.Models.Installation.Requests;
using DotnetManager.Installation.Models.Installation.Targets;
using DotnetManager.Installation.Services.Installation;
using NuGet.Versioning;

namespace DotnetManager.UnitTests.SdkManagement;

public class DotnetInstallOrchestratorTests
{
    [Fact]
    public async Task InstallAsyncDownloadsAndExtractsEverySourceThenFinalizes()
    {
        using var directory = new TestDirectory();
        DotnetDownloadSource[] sources =
        [
            new(new Uri("https://example.test/sdk"), "sdk.tar.gz", "sdk-hash"),
            new(new Uri("https://example.test/runtime"), "runtime.tar.gz", "runtime-hash")
        ];
        var resolver = new StubResolver(sources);
        var downloader = new StubDownloader(directory.Path);
        var extractor = new RecordingExtractor();
        var finalizer = new RecordingFinalizer();
        var orchestrator = new DotnetInstallOrchestrator(resolver, downloader, extractor,
            new StubPathProvider("/managed/dotnet"), finalizer);

        await orchestrator.InstallAsync(CreateRequest(), CancellationToken.None);

        Assert.Equal(["sdk.tar.gz", "runtime.tar.gz"], downloader.Downloaded);
        Assert.Equal(
        [
            (Path.Combine(directory.Path, "sdk.tar.gz"), "/managed/dotnet"),
            (Path.Combine(directory.Path, "runtime.tar.gz"), "/managed/dotnet")
        ], extractor.Extractions);
        Assert.True(finalizer.Called);
    }

    private static InstallRequest CreateRequest()
    {
        return new InstallRequest
        {
            Target = new VersionSelector(NuGetVersion.Parse("10.0.1")),
            Options = new InstallOptions
            {
                Components = [DotnetComponent.Sdk]
            }
        };
    }

    private sealed class StubResolver(IReadOnlyCollection<DotnetDownloadSource> sources)
        : IDotnetInstallResolverService
    {
        public Task<IReadOnlyCollection<DotnetDownloadSource>> ResolveAsync(InstallRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(sources);
        }
    }

    private sealed class StubDownloader(string directory) : IDotnetDownloaderService
    {
        public List<string> Downloaded { get; } = [];

        public Task<DotnetDownload> DownloadAsync(DotnetDownloadSource downloadSource,
            CancellationToken cancellationToken)
        {
            Downloaded.Add(downloadSource.FileName);
            return Task.FromResult(new DotnetDownload(
                Path.Combine(directory, downloadSource.FileName)));
        }
    }

    private sealed class RecordingExtractor : IArchiveExtractorService
    {
        public List<(string Archive, string Destination)> Extractions { get; } = [];

        public Task ExtractAsync(string archivePath,
            string destinationPath,
            CancellationToken cancellationToken)
        {
            Extractions.Add((archivePath, destinationPath));
            return Task.CompletedTask;
        }
    }

    private sealed class StubPathProvider(string path) : IDotnetInstallPathProviderService
    {
        public string GetInstallDirectory()
        {
            return path;
        }

        public string? GetExecutableLinkPath()
        {
            return null;
        }
    }

    private sealed class RecordingFinalizer : IDotnetInstallationFinalizerService
    {
        public bool Called { get; private set; }

        public Task FinalizeAsync(CancellationToken cancellationToken)
        {
            Called = true;
            return Task.CompletedTask;
        }
    }
}
