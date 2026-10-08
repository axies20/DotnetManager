using DotnetManager.Removal.Abstractions;
using DotnetManager.Removal.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.Removal.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRemoval(this IServiceCollection services)
    {
        services.AddTransient<IDotnetRemovalTargetResolverService, DotnetRemovalTargetResolver>();
        services.AddTransient<IDotnetRemovalService, DotnetRemovalService>();

        return services;
    }
}