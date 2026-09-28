using DotnetManager.InstalledDotnet.Abstractions;
using Microsoft.Extensions.Logging;

namespace DotnetManager.InstalledDotnet.Services;

public class DotnetRootLocator(
    IEnumerable<IDotnetRootSource> sources,
    ILogger<DotnetRootLocator> logger) : IDotnetRootLocatorService
{
    public IReadOnlyCollection<string> GetRoots()
    {
        var roots = sources.SelectMany(x => x.DiscoverRoots())
            .Where(Directory.Exists)
            .Select(Path.GetFullPath)
            .Distinct(OperatingSystem.IsWindows()
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal)
            .ToList();

        logger.LogDebug("Discovered {RootCount} .NET root(s): {Roots}",
            roots.Count, string.Join(", ", roots));
        return roots;
    }
}