namespace DotnetManager.UserEnvironment.Helper;

internal static class SystemdDetector
{
    public static bool IsSystemd()
    {
        return OperatingSystem.IsLinux() && Directory.Exists("/run/systemd/system");
    }
}