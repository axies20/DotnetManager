using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using NuGet.Versioning;

namespace DotnetManager.InstalledDotnet.Services;

public class SdkLocator(IDotnetRootLocatorService rootLocator) : IDotnetInstallationLocatorService<SdkInstallation>
{
    public IReadOnlyCollection<SdkInstallation> Find()
    {
        return rootLocator.GetRoots()
            .SelectMany(FindSdks)
            .ToList();
    }

    private static IEnumerable<SdkInstallation> FindSdks(string? root)
    {
        if (root is null)
            return [];

        var sdkRoot = Path.Combine(root, "sdk");

        if (!Directory.Exists(sdkRoot))
            return [];

        return Directory.EnumerateDirectories(sdkRoot)
            .Select(path => new SdkInstallation(NuGetVersion.Parse(Path.GetFileName(path)),
                path));
    }
}
