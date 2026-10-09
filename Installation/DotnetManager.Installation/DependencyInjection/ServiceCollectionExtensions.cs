using DotnetManager.Installation.Abstractions.Archives;
using DotnetManager.Installation.Abstractions.Downloads;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Abstractions.InstallPaths;
using DotnetManager.Installation.InstallPaths;
using DotnetManager.Installation.Services.Archives;
using DotnetManager.Installation.Services.Downloads;
using DotnetManager.Installation.Services.Installation;
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
        services.AddTransient<IDotnetInstallOrchestratorService, DotnetInstallOrchestrator>();

        return services;
    }
}