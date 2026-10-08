namespace DotnetManager.Core.Models;

public static class DotnetPaths
{

    public static string UserInstallRoot => Path.Combine(UserPaths.Home, UserInstallDirectoryName);

    public static string UserToolsRoot => Path.Combine(UserInstallRoot, ToolsDirectoryName);
    public const string UserInstallDirectoryName = ".dotnet";
    public const string ToolsDirectoryName = "tools";
}