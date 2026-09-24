using DotnetManager.Cli;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Cli.Commands;
using DotnetManager.Cli.Commands.Available;
using DotnetManager.Cli.Commands.Available.Abstraction;
using DotnetManager.Cli.Commands.Available.Services;
using DotnetManager.Cli.Commands.Install;
using DotnetManager.Cli.Commands.List;
using DotnetManager.Configuration;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using DotnetManager.InstalledDotnet.RootSources;
using DotnetManager.InstalledDotnet.Services;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Services;
using DotnetManager.SdkManagement.Abstractions.Archives;
using DotnetManager.SdkManagement.Abstractions.Downloads;
using DotnetManager.SdkManagement.Abstractions.Installation;
using DotnetManager.SdkManagement.Abstractions.InstallPaths;
using DotnetManager.SdkManagement.Abstractions.Resolver;
using DotnetManager.SdkManagement.Abstractions.UserEnvironment;
using DotnetManager.SdkManagement.InstallPaths;
using DotnetManager.SdkManagement.Services.Archives;
using DotnetManager.SdkManagement.Services.Downloads;
using DotnetManager.SdkManagement.Services.Installation;
using DotnetManager.SdkManagement.Services.Resolver;
using DotnetManager.SdkManagement.Services.UserEnvironment;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddHttpClient<ISdkManifestProvider, SdkManifestProvider>();
        services.AddSingleton<ISdkReleaseService, SdkReleaseService>();
    }

    private static void AddSdkManagement(IServiceCollection services)
    {
        services.AddHttpClient<IDotnetDownloader, DotnetDownloader>();
        services.AddSingleton<IArchiveExtractor, ArchiveExtractor>();
        services.AddSingleton<IDotnetInstallPathProvider, UnixDotnetInstallPathProvider>();
        services.AddSingleton<IUserEnvironmentConfigurator, UnixUserEnvironmentConfigurator>();
        services.AddSingleton<IDotnetInstallationFinalizer, DotnetInstallationFinalizer>();
        services.AddTransient<IDotnetInstallResolver, DotnetInstallResolver>();
        services.AddTransient<IDotnetInstallOrchestrator, DotnetInstallOrchestrator>();
    }

    private static void AddInstalledDotnet(IServiceCollection services)
    {
        services.AddSingleton<IDotnetRootSource, EnvironmentDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, PathDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, UnixInstallLocationDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, WindowsRegistryDotnetRootSource>();
        services.AddSingleton<IDotnetRootLocator, DotnetRootLocator>();

        services.AddSingleton<IDotnetInstallationLocator<SdkInstallation>, SdkLocator>();
        services.AddSingleton<IDotnetInstallationLocator<RuntimeInstallation>, RuntimeLocator>();
        services.AddSingleton<IDotnetInstallationLocator<HostInstallation>, HostLocator>();
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