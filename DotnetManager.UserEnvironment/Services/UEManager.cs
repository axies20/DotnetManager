using DotnetManager.UserEnvironment.Abstraction;
using Microsoft.Extensions.Logging;

namespace DotnetManager.UserEnvironment.Services;

internal sealed class UEManager(IUEResolver resolver, ILogger<UEManager> logger) : IUEManager
{
    public async Task ConfigureEnvironment(CancellationToken cancellationToken)
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
                cancellationToken);
        }

        logger.LogInformation("User environment configuration completed");
    }
}