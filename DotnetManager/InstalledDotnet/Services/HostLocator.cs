using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using NuGet.Versioning;

namespace DotnetManager.InstalledDotnet.Services;

public class HostLocator(IDotnetRootLocator rootLocator) : IDotnetInstallationLocator<HostInstallation>
{
    public IReadOnlyCollection<HostInstallation> Find()
    {
        return rootLocator.GetRoots()
            .SelectMany(FindHosts)
            .ToList();
    }

    private static IEnumerable<HostInstallation> FindHosts(string? root)
    {
        if (root is null)
            return [];

        var hostRoot = Path.Combine(root, "host", "fxr");

        if (!Directory.Exists(hostRoot))
            return [];

        return Directory.EnumerateDirectories(hostRoot)
            .Select(path => new HostInstallation(NuGetVersion.Parse(Path.GetFileName(path)),
                path));
    }
}