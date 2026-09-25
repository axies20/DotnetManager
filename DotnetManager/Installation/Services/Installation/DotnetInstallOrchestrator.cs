using DotnetManager.Installation.Abstractions.Archives;
using DotnetManager.Installation.Abstractions.Downloads;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Abstractions.InstallPaths;
using DotnetManager.Installation.Abstractions.Resolver;
using DotnetManager.Installation.Models.Installation.Requests;

namespace DotnetManager.Installation.Services.Installation;

public sealed class DotnetInstallOrchestrator : IDotnetInstallOrchestratorService
{
    private readonly IDotnetInstallResolverService _resolver;
    private readonly IDotnetDownloaderService _downloader;
    private readonly IArchiveExtractorService _extractor;
    private readonly IDotnetInstallPathProviderService _pathProvider;
    private readonly IDotnetInstallationFinalizerService _finalizer;

    public DotnetInstallOrchestrator(IDotnetInstallResolverService resolver,
        IDotnetDownloaderService downloader,
        IArchiveExtractorService extractor,
        IDotnetInstallPathProviderService pathProvider,
        IDotnetInstallationFinalizerService finalizer)
    {
        _resolver = resolver;
        _downloader = downloader;
        _extractor = extractor;
        _pathProvider = pathProvider;
        _finalizer = finalizer;
    }

    public async Task InstallAsync(InstallRequest request, CancellationToken cancellationToken)
    {
        var resolveUri = await _resolver.ResolveAsync(request, cancellationToken);
        var path = _pathProvider.GetInstallDirectory();

        foreach (var uri in resolveUri)
        {
            var downloadData = await _downloader.DownloadAsync(uri, cancellationToken);

            await _extractor.ExtractAsync(downloadData.FilePath, path, cancellationToken);
        }

        await _finalizer.FinalizeAsync(cancellationToken);
    }
}
