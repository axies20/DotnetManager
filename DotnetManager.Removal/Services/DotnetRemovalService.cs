using DotnetManager.Removal.Abstractions;
using DotnetManager.Core.Models;
using Microsoft.Extensions.Logging;

namespace DotnetManager.Removal.Services;

internal sealed class DotnetRemovalService(
    IDotnetRemovalTargetResolverService resolver,
    ILogger<DotnetRemovalService> logger) : IDotnetRemovalService
{

    public void RemoveAsync(string dotnetVersion)
    {
        logger.LogInformation("Removing all .NET components matching {Version}", dotnetVersion);

        foreach (var component in Enum.GetValues<DotnetComponent>())
        {
            RemoveAsync(dotnetVersion, component);
        }
    }


    public void RemoveAsync(string dotnetVersion, DotnetComponent component)
    {
        var paths = resolver.Resolve(dotnetVersion, component);

        if (paths.Count == 0)
        {
            logger.LogInformation("No {Component} installations match {Version}",
                component, dotnetVersion);
            return;
        }

        logger.LogInformation("Removing {Count} {Component} installation(s) matching {Version}",
            paths.Count, component, dotnetVersion);

        foreach (var path in paths)
        {
            logger.LogInformation("Deleting installation directory {Path}", path);
            Directory.Delete(path, true);
        }

        logger.LogInformation("Removed {Component} installations matching {Version}",
            component, dotnetVersion);
    }
}