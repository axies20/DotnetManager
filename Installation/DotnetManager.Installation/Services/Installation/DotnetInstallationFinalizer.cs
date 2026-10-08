using DotnetManager.Installation.Exceptions;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Abstractions.InstallPaths;
using DotnetManager.UserEnvironment.Abstraction;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Installation.Services.Installation;

internal sealed class DotnetInstallationFinalizer : IDotnetInstallationFinalizerService
{
    private readonly IDotnetInstallPathProviderService _pathProvider;
    private readonly IUEManager _ueManager;
    private readonly ILogger<DotnetInstallationFinalizer> _logger;

    public DotnetInstallationFinalizer(IDotnetInstallPathProviderService pathProvider,
        ILogger<DotnetInstallationFinalizer> logger,
        IUEManager ueManager)
    {
        _pathProvider = pathProvider;
        _logger = logger;
        _ueManager = ueManager;
    }

    public async Task FinalizeAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var installRoot = _pathProvider.GetInstallDirectory();
        var executable = Path.Combine(installRoot, "dotnet");
        _logger.LogInformation("Finalizing installation in {InstallRoot}", installRoot);

        if (!File.Exists(executable))
        {
            _logger.LogWarning("The dotnet executable was not found in {InstallRoot}", installRoot);
            throw new InstalledDotnetExecutableNotFoundException(installRoot);
        }

        await _ueManager.ConfigureEnvironment(cancellationToken);
        _logger.LogInformation("Installation finalization completed");
    }
}