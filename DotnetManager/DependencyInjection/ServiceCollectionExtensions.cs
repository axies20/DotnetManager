using DotnetManager.Cli;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Cli.Commands;
using DotnetManager.Cli.Commands.Available;
using DotnetManager.Cli.Commands.Available.Abstraction;
using DotnetManager.Cli.Commands.Available.Services;
using DotnetManager.Configuration;
using DotnetManager.Installation.Abstractions.Archives;
using DotnetManager.Installation.Abstractions.Downloads;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Abstractions.InstallPaths;
using DotnetManager.Installation.Abstractions.Removal;
using DotnetManager.Installation.Abstractions.Resolver;
using DotnetManager.Installation.Abstractions.UserEnvironment;
using DotnetManager.Installation.Services.Archives;
using DotnetManager.Installation.Services.Downloads;
using DotnetManager.Installation.Services.Installation;
using DotnetManager.Installation.Services.Removal;
using DotnetManager.Installation.Services.Resolver;
using DotnetManager.Installation.Services.UserEnvironment;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using DotnetManager.InstalledDotnet.RootSources;
using DotnetManager.InstalledDotnet.Services;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Services;
using Microsoft.Extensions.DependencyInjection;
using UnixDotnetInstallPathProvider = DotnetManager.Installation.InstallPaths.UnixDotnetInstallPathProvider;

namespace DotnetManager.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDotnetManager(this IServiceCollection services)
    {
        AddConfiguration(services);
        AddReleaseMetadata(services);
        AddSdkManagement(services);
        AddInstalledDotnet(services);
        AddCli(services);

        return services;
    }

    private static void AddConfiguration(IServiceCollection services)
    {
        services.AddOptions<DotnetManagerOptions>()
            .BindConfiguration("DotnetManager")
            .ValidateOnStart();
    }

    private static void AddReleaseMetadata(IServiceCollection services)
    {
        services.AddHttpClient<ISdkManifestProviderService, SdkManifestProvider>();
        services.AddSingleton<ISdkReleaseService, SdkReleaseService>();
    }

    private static void AddSdkManagement(IServiceCollection services)
    {
        services.AddHttpClient<IDotnetDownloaderService, DotnetDownloader>();
        services.AddSingleton<IArchiveExtractorService, ArchiveExtractor>();
        services.AddSingleton<IDotnetInstallPathProviderService, UnixDotnetInstallPathProvider>();
        services.AddSingleton<IUserEnvironmentConfiguratorService, UnixUserEnvironmentConfigurator>();
        services.AddSingleton<IDotnetInstallationFinalizerService, DotnetInstallationFinalizer>();
        services.AddTransient<IDotnetInstallResolverService, DotnetInstallResolver>();
        services.AddTransient<IDotnetInstallOrchestratorService, DotnetInstallOrchestrator>();
        services.AddTransient<IDotnetRemovalService, DotnetRemovalService>();
    }

    private static void AddInstalledDotnet(IServiceCollection services)
    {
        services.AddSingleton<IDotnetRootSource, EnvironmentDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, PathDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, UnixInstallLocationDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, WindowsRegistryDotnetRootSource>();
        services.AddSingleton<IDotnetRootLocatorService, DotnetRootLocator>();

        services.AddSingleton<IDotnetInstallationLocatorService<SdkInstallation>, SdkLocator>();
        services.AddSingleton<IDotnetInstallationLocatorService<RuntimeInstallation>, RuntimeLocator>();
        services.AddSingleton<IDotnetInstallationLocatorService<HostInstallation>, HostLocator>();
    }

    private static void AddCli(IServiceCollection services)
    {
        services.AddTransient<ICommand, AvailableCommand>();
        services.AddTransient<ICommand, InstallCommand>();
        services.AddTransient<ICommand, ListCommand>();
        services.AddTransient<ICommand, RemoveCommand>();
        services.AddTransient<ICommand, UpdateCommand>();

        services.AddSingleton<CommandRegistration>();
    }
}