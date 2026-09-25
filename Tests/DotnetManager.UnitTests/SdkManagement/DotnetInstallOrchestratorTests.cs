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
        var resolver = new OrchestratorStubResolver(sources);
        var downloader = new OrchestratorStubDownloader(directory.Path);
        var extractor = new OrchestratorRecordingExtractor();
        var finalizer = new OrchestratorRecordingFinalizer();
        var orchestrator = new DotnetInstallOrchestrator(resolver, downloader, extractor,
            new OrchestratorStubPathProvider("/managed/dotnet"), finalizer);

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

}
