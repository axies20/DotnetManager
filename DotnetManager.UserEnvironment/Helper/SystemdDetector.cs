namespace DotnetManager.UserEnvironment.Helper;

public static class SystemdDetector
{
    public static bool IsSystemd()
    {
        return OperatingSystem.IsLinux() && Directory.Exists("/run/systemd/system");
    }
}