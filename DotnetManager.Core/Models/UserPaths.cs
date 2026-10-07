namespace DotnetManager.Core.Models;

public static class UserPaths
{

    public static string Home => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public static string XdgConfigHome
    {
        get
        {
            var path = Environment.GetEnvironmentVariable(XdgConfigHomeVariable);

            return !string.IsNullOrWhiteSpace(path) && Path.IsPathFullyQualified(path)
                ? path
                : Path.Combine(Home, DefaultConfigDirectoryName);
        }
    }

    private const string XdgConfigHomeVariable = "XDG_CONFIG_HOME";
    private const string DefaultConfigDirectoryName = ".config";
}