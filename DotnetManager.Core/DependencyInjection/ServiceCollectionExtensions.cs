using DotnetManager.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddOptions<DotnetManagerOptions>()
            .BindConfiguration("DotnetManager")
            .ValidateOnStart();

        return services;
    }
}