using DotnetManager.Cli;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Cli.Commands;
using DotnetManager.Cli.Commands.Available;
using DotnetManager.Cli.Commands.List;
using DotnetManager.Configuration;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using DotnetManager.InstalledDotnet.RootSources;
using DotnetManager.InstalledDotnet.Services;
using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Services;
using DotnetManager.SdkManagement.Abstractions;
using DotnetManager.SdkManagement.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDotnetManager(this IServiceCollection services)
    {
        services.AddOptions<DotnetManagerOptions>()
            .BindConfiguration("DotnetManager")
            .ValidateOnStart();

        services.AddHttpClient<ISdkDownloader, SdkDownloader>();
        services.AddHttpClient<ISdkManifestProvider, SdkManifestProvider>();
        services.AddSingleton<ISdkInstaller, SdkInstaller>();

        services.AddSingleton<IDotnetRootSource, EnvironmentDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, PathDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, UnixInstallLocationDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, WindowsRegistryDotnetRootSource>();
        services.AddSingleton<IDotnetRootLocator, DotnetRootLocator>();

        services.AddSingleton<IDotnetInstallationLocator<SdkInstallation>, SdkLocator>();
        services.AddSingleton<IDotnetInstallationLocator<RuntimeInstallation>, RuntimeLocator>();
        services.AddSingleton<IDotnetInstallationLocator<HostInstallation>, HostLocator>();

        services.AddTransient<ICommand, AvailableCommand>();
        services.AddTransient<ICommand, InstallCommand>();
        services.AddTransient<ICommand, ListCommand>();
        services.AddTransient<ICommand, RemoveCommand>();
        services.AddTransient<ICommand, UpdateCommand>();

        services.AddSingleton<CommandRegistration>();
        return services;
    }
}