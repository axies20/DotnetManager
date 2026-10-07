using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using DotnetManager.InstalledDotnet.RootSources;
using DotnetManager.InstalledDotnet.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.InstalledDotnet.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInstalledDotnet(this IServiceCollection services)
    {
        services.AddSingleton<IDotnetRootSource, EnvironmentDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, PathDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, UnixInstallLocationDotnetRootSource>();
        services.AddSingleton<IDotnetRootSource, WindowsRegistryDotnetRootSource>();
        services.AddSingleton<IDotnetRootLocatorService, DotnetRootLocator>();

        services.AddSingleton<IDotnetInstallationLocatorService<SdkInstallation>, SdkLocator>();
        services.AddSingleton<IDotnetInstallationLocatorService<RuntimeInstallation>, RuntimeLocator>();
        services.AddSingleton<IDotnetInstallationLocatorService<HostInstallation>, HostLocator>();

        return services;
    }
}