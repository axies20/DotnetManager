using DotnetManager.SdkManagement.Abstractions.Archives;
using DotnetManager.SdkManagement.Abstractions.Downloads;
using DotnetManager.SdkManagement.Abstractions.Installation;
using DotnetManager.SdkManagement.Abstractions.InstallPaths;
using DotnetManager.SdkManagement.Abstractions.Resolver;
using DotnetManager.SdkManagement.Models.Installation.Requests;

namespace DotnetManager.SdkManagement.Services.Installation;

public sealed class DotnetInstallOrchestrator : IDotnetInstallOrchestrator
{
    private readonly IDotnetInstallResolver _resolver;
    private readonly IDotnetDownloader _downloader;
    private readonly IArchiveExtractor _extractor;
    private readonly IDotnetInstallPathProvider _pathProvider;
    private readonly IDotnetInstallationFinalizer _finalizer;

    public DotnetInstallOrchestrator(IDotnetInstallResolver resolver,
        IDotnetDownloader downloader,
        IArchiveExtractor extractor,
        IDotnetInstallPathProvider pathProvider,
        IDotnetInstallationFinalizer finalizer)
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