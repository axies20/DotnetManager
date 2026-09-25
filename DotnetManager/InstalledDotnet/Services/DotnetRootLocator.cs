using DotnetManager.InstalledDotnet.Abstractions;

namespace DotnetManager.InstalledDotnet.Services;

public class DotnetRootLocator(IEnumerable<IDotnetRootSource> sources) : IDotnetRootLocatorService
{
    public IReadOnlyCollection<string> GetRoots()
    {
        return sources.SelectMany(x => x.DiscoverRoots())
            .Where(Directory.Exists)
            .Select(Path.GetFullPath)
            .Distinct(OperatingSystem.IsWindows()
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal)
            .ToList();
    }
}
