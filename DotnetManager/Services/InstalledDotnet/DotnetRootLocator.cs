using DotnetManager.Abstraction.InstalledDotnet;

namespace DotnetManager.Services.InstalledDotnet;

public class DotnetRootLocator(IEnumerable<IDotnetRootSource> sources) : IDotnetRootLocator
{
    public IReadOnlyCollection<string> GetRoots()
    {
        return sources
            .SelectMany(x => x.DiscoverRoots())
            .Where(Directory.Exists)
            .Select(Path.GetFullPath)
            .Distinct(OperatingSystem.IsWindows()
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal)
            .ToList();
    }
}