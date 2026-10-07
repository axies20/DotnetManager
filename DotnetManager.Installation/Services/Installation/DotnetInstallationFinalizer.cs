using DotnetManager.Installation.Exceptions;
using DotnetManager.Installation.Abstractions.Installation;
using DotnetManager.Installation.Abstractions.InstallPaths;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Installation.Services.Installation;

internal sealed class DotnetInstallationFinalizer : IDotnetInstallationFinalizerService
{
    private readonly IDotnetInstallPathProviderService _pathProvider;
    private readonly ILogger<DotnetInstallationFinalizer> _logger;

    public DotnetInstallationFinalizer(IDotnetInstallPathProviderService pathProvider,
        ILogger<DotnetInstallationFinalizer> logger)
    {
        _pathProvider = pathProvider;
        _logger = logger;
    }

    public Task FinalizeAsync(CancellationToken cancellationToken)
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

        _logger.LogInformation("Installation finalization completed");
        return Task.CompletedTask;
    }
}
