using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;
using Microsoft.Extensions.Logging;

namespace DotnetManager.UserEnvironment.Services;

internal sealed class UEManager(IUEResolver resolver, ILogger<UEManager> logger) : IUEManager
{
    public async Task ConfigureEnvironment()
    {
        var configurations = resolver.Resolve();

        if (configurations.Count == 0)
        {
            logger.LogWarning("No supported user environment configuration was detected");
            return;
        }

        logger.LogInformation("Configuring {ConfigurationCount} user environment target(s)",
            configurations.Count);

        foreach (var resolvedConfiguration in configurations)
        {
            await resolvedConfiguration.Configurator.ConfigureAsync(resolvedConfiguration.Path,
                CancellationToken.None);
        }

        logger.LogInformation("User environment configuration completed");
    }

    public void RemoveEnvironment()
    {
        var configurations = resolver.Resolve();

        if (configurations.Count == 0)
        {
            logger.LogWarning("No supported user environment configuration was detected");
            return;
        }

        logger.LogInformation("Removing {ConfigurationCount} user environment configuration(s)",
            configurations.Count);

        foreach (var resolvedConfiguration in configurations)
        {
            resolvedConfiguration.Configurator.Remove(resolvedConfiguration.Path);
        }

        logger.LogInformation("User environment configuration removal completed");
    }

    public void RemoveEnvironment(UEKind kind)
    {
        var config = resolver.Resolve().FirstOrDefault(x => x.Configurator.Kind == kind);

        if (config is null)
        {
            logger.LogWarning("No {Environment} environment configuration target was detected", kind);
            return;
        }

        config.Configurator.Remove(config.Path);
    }
}
