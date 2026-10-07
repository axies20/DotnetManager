using DotnetManager.Installation.Abstractions.InstallPaths;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Installation.InstallPaths;

internal sealed class UserDotnetInstallPathProvider(ILogger<UserDotnetInstallPathProvider> logger)
    : IDotnetInstallPathProviderService
{
    public string GetInstallDirectory()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var installDirectory = Path.Combine(home, ".dotnet");

        logger.LogDebug("Using user install directory {InstallDirectory}", installDirectory);
        return installDirectory;
    }
}
