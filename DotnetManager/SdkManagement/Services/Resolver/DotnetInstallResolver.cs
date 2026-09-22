using DotnetManager.SdkManagement.Abstractions.Downloads;
using DotnetManager.SdkManagement.Abstractions.Resolver;
using DotnetManager.SdkManagement.Models.Installation;

namespace DotnetManager.SdkManagement.Services.Resolver;

public class DotnetInstallResolver : IDotnetInstallResolver
{
    private readonly IDotnetDownloader _downloader;
    private readonly DotnetInstallPlanner _planner;

    public DotnetInstallResolver(IDotnetDownloader downloader, DotnetInstallPlanner planner)
    {
        _downloader = downloader;
        _planner = planner;
    }

    public async Task ResolveAsync(InstallRequest request, CancellationToken cancellationToken)
    {
        var sources = await _planner.CreateAsync(request, cancellationToken);

        foreach (var source in sources)
            await _downloader.DownloadAsync(source, cancellationToken);
    }
}
