using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using NuGet.Versioning;

namespace DotnetManager.InstalledDotnet.Services;

public class RuntimeLocator(IDotnetRootLocator rootLocator) : IDotnetInstallationLocator<RuntimeInstallation>
{
    public IReadOnlyCollection<RuntimeInstallation> Find()
    {
        return rootLocator.GetRoots()
            .SelectMany(FindRuntimes)
            .ToList();
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
                        framework, NuGetVersion.Parse(Path.GetFileName(versionPath)),
                        versionPath));
            });
    }
}