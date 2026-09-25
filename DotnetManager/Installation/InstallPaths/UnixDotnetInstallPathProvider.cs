using System.Runtime.InteropServices;
using DotnetManager.Installation.Abstractions.InstallPaths;

namespace DotnetManager.Installation.InstallPaths;

public partial class UnixDotnetInstallPathProvider : IDotnetInstallPathProviderService
{
    public string GetInstallDirectory()
    {
        if (IsRoot())
        {
            return "/usr/local/share/dotnet";
        }

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        return Path.Combine(home, ".dotnet");
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
