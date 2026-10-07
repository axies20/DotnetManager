namespace DotnetManager.Core.Models;

public static class DotnetPaths
{
    public const string UserInstallDirectoryName = ".dotnet";
    public const string ToolsDirectoryName = "tools";

    public static string UserInstallRoot => Path.Combine(UserPaths.Home, UserInstallDirectoryName);

    public static string UserToolsRoot => Path.Combine(UserInstallRoot, ToolsDirectoryName);
}
