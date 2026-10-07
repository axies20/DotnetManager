using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Services.Configuration;
using DotnetManager.UserEnvironment.Services.Resolver;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.UserEnvironment.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserEnvironment(this IServiceCollection services)
    {
        services.AddSingleton<IUEConfigurator, FishConfig>();
        services.AddSingleton<IUEConfigurator, OhMyZshUEConfig>();
        services.AddSingleton<IUEConfigurator, EnvironmentDUEConfig>();
        services.AddSingleton<IUEResolver, UEResolver>();

        return services;
    }
}