using DotnetManager.Exception.Installation;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Abstractions.InstallPaths;
using DotnetManager.Installation.Abstractions.UserEnvironment;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Installation.Services.Installation;

public class DotnetInstallationFinalizer : IDotnetInstallationFinalizerService
{
    private readonly IDotnetInstallPathProviderService _pathProvider;
    private readonly IUserEnvironmentConfiguratorService _userEnvironmentConfigurator;
    private readonly ILogger<DotnetInstallationFinalizer> _logger;

    public DotnetInstallationFinalizer(IDotnetInstallPathProviderService pathProvider,
        IUserEnvironmentConfiguratorService userEnvironmentConfigurator,
        ILogger<DotnetInstallationFinalizer> logger)
    {
        _pathProvider = pathProvider;
        _userEnvironmentConfigurator = userEnvironmentConfigurator;
        _logger = logger;
    }

    public async Task FinalizeAsync(CancellationToken cancellationToken)
    {
        var installRoot = _pathProvider.GetInstallDirectory();
        var executable = Path.Combine(installRoot, "dotnet");
        _logger.LogInformation("Finalizing installation in {InstallRoot}", installRoot);

        if (!File.Exists(executable))
        {
            _logger.LogWarning("The dotnet executable was not found in {InstallRoot}", installRoot);
            throw new InstalledDotnetExecutableNotFoundException(installRoot);
        }

        var linkPath = _pathProvider.GetExecutableLinkPath();

        if (linkPath is not null)
        {
            EnsureExecutableLink(linkPath, executable);
            _logger.LogInformation("Installation finalization completed");
            return;
        }

        _logger.LogInformation("Configuring the user environment for {InstallRoot}", installRoot);
        await _userEnvironmentConfigurator.ConfigureAsync(installRoot, cancellationToken);
        _logger.LogInformation("Installation finalization completed");
    }

    private void EnsureExecutableLink(string linkPath, string targetPath)
    {
        var link = new FileInfo(linkPath);
        _logger.LogDebug("Ensuring executable link at {LinkPath}", linkPath);

        if (link.Exists || link.LinkTarget is not null)
        {
            _logger.LogDebug("Executable link already exists at {LinkPath}", linkPath);
            return;
        }

        _logger.LogInformation("Creating executable link {LinkPath} -> {TargetPath}",
            linkPath, targetPath);
        File.CreateSymbolicLink(linkPath, targetPath);
    }
}