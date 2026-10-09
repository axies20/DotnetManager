using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;
using Microsoft.Extensions.Logging;

namespace DotnetManager.UserEnvironment.Services;

internal sealed class UEManager(IUEResolver resolver, ILogger<UEManager> logger) : IUEManager
{
    internal const string UnixManualShellConfigurationInstructions = """
                                                                     No automatically configurable shell environment was detected.
                                                                     Configure .NET for your shell manually by copying one of the following blocks.

                                                                     Zsh - add to ~/.zshrc:
                                                                     export DOTNET_ROOT="$HOME/.dotnet"
                                                                     export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"

                                                                     Then run: source ~/.zshrc

                                                                     Bash - add to ~/.bashrc:
                                                                     export DOTNET_ROOT="$HOME/.dotnet"
                                                                     export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"

                                                                     Then run: source ~/.bashrc
                                                                     """;

    public async Task ConfigureEnvironment(CancellationToken cancellationToken)
    {
        var configurations = resolver.Resolve();
        // TODO: Use IHostPlatformService.IsUnix instead of direct operating-system checks.
        var isUnix = OperatingSystem.IsLinux() || OperatingSystem.IsMacOS();
        var requiresManualShellConfiguration = RequiresManualShellConfiguration(configurations);

        if (configurations.Count == 0)
        {
            if (isUnix && requiresManualShellConfiguration)
                logger.LogWarning(UnixManualShellConfigurationInstructions);
            else
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

        if (isUnix && requiresManualShellConfiguration)
            logger.LogWarning(UnixManualShellConfigurationInstructions);
    }

    public void RemoveEnvironment()
    {
        var configurations = resolver.Resolve();

        foreach (var resolvedConfiguration in configurations)
        {
            resolvedConfiguration.Configurator.Remove(resolvedConfiguration.Path);
        }
    }

    internal static bool RequiresManualShellConfiguration(IReadOnlyCollection<UEResolvedConfiguration> configurations)
    {
        return configurations.All(x => x.Configurator.Kind == UEKind.EnvironmentD);
    }
}