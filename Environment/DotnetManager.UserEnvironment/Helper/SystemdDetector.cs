namespace DotnetManager.UserEnvironment.Helper;

internal static class SystemdDetector
{
    public static bool IsSystemd()
    {
        // TODO: Use IHostPlatformService.IsLinux for the platform check.
        return OperatingSystem.IsLinux() && Directory.Exists("/run/systemd/system");
    }
}