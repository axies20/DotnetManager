using System.Runtime.InteropServices;
using DotnetManager.Installation.Abstractions.InstallPaths;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Installation.InstallPaths;

public partial class UnixDotnetInstallPathProvider(ILogger<UnixDotnetInstallPathProvider> logger)
    : IDotnetInstallPathProviderService
{
    public string GetInstallDirectory()
    {
        if (IsRoot())
        {
            logger.LogDebug("Root user detected; using install directory {InstallDirectory}",
                "/usr/local/share/dotnet");
            return "/usr/local/share/dotnet";
        }

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var installDirectory = Path.Combine(home, ".dotnet");
        logger.LogDebug("Non-root user detected; using install directory {InstallDirectory}",
            installDirectory);
        return installDirectory;
    }

    public string? GetExecutableLinkPath()
    {
        return IsRoot() ? "/usr/local/bin/dotnet" : null;
    }

    private static bool IsRoot()
    {
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
        {
            return false;
        }

        return GetEffectiveUserId() == 0;
    }


    [LibraryImport("libc", EntryPoint = "geteuid")]
    private static partial uint GetEffectiveUserId();
}