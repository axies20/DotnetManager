using System.Diagnostics;
using DotnetManager.Installation.Abstractions.Archives;
using DotnetManager.Installation.Abstractions.Downloads;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Abstractions.InstallPaths;
using DotnetManager.Installation.Abstractions.Resolver;
using DotnetManager.Installation.Models.Installation.Requests;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Installation.Services.Installation;

internal sealed class DotnetInstallOrchestrator : IDotnetInstallOrchestratorService
{
    private readonly IDotnetInstallResolverService _resolver;
    private readonly IDotnetDownloaderService _downloader;
    private readonly IArchiveExtractorService _extractor;
    private readonly IDotnetInstallPathProviderService _pathProvider;
    private readonly IDotnetInstallationFinalizerService _finalizer;
    private readonly ILogger<DotnetInstallOrchestrator> _logger;

    public DotnetInstallOrchestrator(IDotnetInstallResolverService resolver,
        IDotnetDownloaderService downloader,
        IArchiveExtractorService extractor,
        IDotnetInstallPathProviderService pathProvider,
        IDotnetInstallationFinalizerService finalizer,
        ILogger<DotnetInstallOrchestrator> logger)
    {
        _resolver = resolver;
        _downloader = downloader;
        _extractor = extractor;
        _pathProvider = pathProvider;
        _finalizer = finalizer;
        _logger = logger;
    }

    public async Task InstallAsync(InstallRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting .NET installation for {TargetType}; components: {Components}",
            request.Target.GetType().Name, string.Join(", ", request.Options.Components));

        var resolveUri = await _resolver.ResolveAsync(request, cancellationToken);
        var path = _pathProvider.GetInstallDirectory();
        _logger.LogInformation("Resolved {ArtifactCount} artifact(s); installing to {InstallDirectory}",
            resolveUri.Count,
            path);

        foreach (var uri in resolveUri)
        {
            var downloadData = await _downloader.DownloadAsync(uri, cancellationToken);

            await _extractor.ExtractAsync(downloadData.FilePath, path, cancellationToken);
        }

        await _finalizer.FinalizeAsync(cancellationToken);
        _logger.LogInformation("\nInstallation completed successfully in {ElapsedSeconds:F1} seconds",
            stopwatch.Elapsed.TotalSeconds);
    }
}
