using DotnetManager.Cli.DependencyInjection;
using DotnetManager.Installation.DependencyInjection;
using DotnetManager.InstalledDotnet.DependencyInjection;
using DotnetManager.ReleaseMetadata.DependencyInjection;
using DotnetManager.Removal.DependencyInjection;
using DotnetManager.UserEnvironment.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDotnetManager(this IServiceCollection services)
    {
        return services
            .AddReleaseMetadata()
            .AddUserEnvironment()
            .AddInstallation()
            .AddInstalledDotnet()
            .AddRemoval()
            .AddCli();
    }
}