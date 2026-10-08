using DotnetManager.Installation.Abstractions.Archives;
using DotnetManager.Installation.Abstractions.Downloads;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Abstractions.InstallPaths;
using DotnetManager.Installation.Abstractions.Resolver;
using DotnetManager.Installation.InstallPaths;
using DotnetManager.Installation.Models.Installation.Targets;
using DotnetManager.Installation.Services.Archives;
using DotnetManager.Installation.Services.Downloads;
using DotnetManager.Installation.Services.Installation;
using DotnetManager.Installation.Services.Resolver;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.Installation.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInstallation(this IServiceCollection services)
    {
        services.AddHttpClient<IDotnetDownloaderService, DotnetDownloader>();
        services.AddSingleton<IArchiveExtractorService, ArchiveExtractor>();
        services.AddSingleton<IDotnetInstallPathProviderService, UserDotnetInstallPathProvider>();
        services.AddSingleton<IDotnetInstallationFinalizerService, DotnetInstallationFinalizer>();
        services.AddTransient<IInstallReleaseResolverService<LatestSelector>, LatestInstallResolver>();
        services.AddTransient<IInstallReleaseResolverService<VersionSelector>, VersionInstallResolver>();
        services.AddTransient<IDotnetInstallResolverService, DotnetInstallResolver>();
        services.AddTransient<IDotnetInstallOrchestratorService, DotnetInstallOrchestrator>();

        return services;
    }
}
