using DotnetManager.Core.Models;
using DotnetManager.Installation.Abstractions.InstallPaths;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Installation.InstallPaths;

internal sealed class UserDotnetInstallPathProvider(ILogger<UserDotnetInstallPathProvider> logger)
    : IDotnetInstallPathProviderService
{
    public string GetInstallDirectory()
    {
        var installDirectory = DotnetPaths.UserInstallRoot;

        logger.LogDebug("Using user install directory {InstallDirectory}", installDirectory);
        return installDirectory;
    }
}