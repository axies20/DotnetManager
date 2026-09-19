using DotnetManager.Models;
using NuGet.Versioning;

namespace DotnetManager.Services.InstalledDotnet;

public class DotnetInstallationLocator
{


   

    private static IEnumerable<string> GetPathDirs(string path)
    {
        var directories = path?.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (directories is null)
            return [];

        var executableName = OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet";

        return directories.Select(directory => Path.Combine(directory, executableName))
            .Where(File.Exists)
            .ToList();
    }

    private static IEnumerable<SdkInstallation> FindSdks(string? root)
    {
        if (root is null)
            return [];

        var sdkRoot = Path.Combine(root, "sdk");

        if (!Directory.Exists(sdkRoot))
            return [];

        return Directory
            .EnumerateDirectories(sdkRoot)
            .Select(path => new SdkInstallation(NuGetVersion.Parse(Path.GetFileName(path)),
                path));
    }

    private static IEnumerable<RuntimeInstallation> FindRuntimes(string? root)
    {
        if (root is null)
            return [];

        var sharedRoot = Path.Combine(root, "shared");

        if (!Directory.Exists(sharedRoot))
            return [];

        return Directory
            .EnumerateDirectories(sharedRoot)
            .SelectMany(frameworkPath =>
            {
                var framework = Path.GetFileName(frameworkPath);

                return Directory
                    .EnumerateDirectories(frameworkPath)
                    .Select(versionPath => new RuntimeInstallation(
                        framework,
                        NuGetVersion.Parse(Path.GetFileName(versionPath)),
                        versionPath));
            });
    }

    private static IEnumerable<HostInstallation> FindHosts(string? root)
    {
        if (root is null)
            return [];

        var hostRoot = Path.Combine(root, "host", "fxr");

        if (!Directory.Exists(hostRoot))
            return [];

        return Directory
            .EnumerateDirectories(hostRoot)
            .Select(path => new HostInstallation(
                NuGetVersion.Parse(Path.GetFileName(path)),
                path));
    }
}