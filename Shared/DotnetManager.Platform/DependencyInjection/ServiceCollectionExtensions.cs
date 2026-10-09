using DotnetManager.Platform.Abstractions;
using DotnetManager.Platform.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.Platform.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlatform(this IServiceCollection services)
    {
        services.AddSingleton<IHostPlatformService, HostPlatformService>();

        return services;
    }
}
