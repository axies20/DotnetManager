using DotnetManager.ReleaseMetadata.Abstractions;
using DotnetManager.ReleaseMetadata.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.ReleaseMetadata.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReleaseMetadata(this IServiceCollection services)
    {
        services.AddHttpClient<ISdkManifestProviderService, SdkManifestProvider>();

        return services;
    }
}
