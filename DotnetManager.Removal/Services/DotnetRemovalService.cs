using DotnetManager.Removal.Abstractions;
using DotnetManager.Core.Models;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace DotnetManager.Removal.Services;

public class DotnetRemovalService : IDotnetRemovalService
{
    private readonly IDotnetInstallationLocatorService<HostInstallation> _hostLocator;
    private readonly IDotnetInstallationLocatorService<RuntimeInstallation> _runtimeLocator;
    private readonly IDotnetInstallationLocatorService<SdkInstallation> _sdkLocator;
    private readonly ILogger<DotnetRemovalService> _logger;


    public DotnetRemovalService(IDotnetInstallationLocatorService<HostInstallation> hostLocator,
        IDotnetInstallationLocatorService<RuntimeInstallation> runtimeLocator,
        IDotnetInstallationLocatorService<SdkInstallation> sdkLocator,
        ILogger<DotnetRemovalService> logger)
    {
        _hostLocator = hostLocator;
        _runtimeLocator = runtimeLocator;
        _sdkLocator = sdkLocator;
        _logger = logger;
    }


    public void RemoveAsync(string dotnetVersion)
    {
        _logger.LogInformation("Removing all .NET components matching {Version}", dotnetVersion);

        foreach (var component in Enum.GetValues<DotnetComponent>())
        {
            RemoveAsync(dotnetVersion, component);
        }
    }


    public void RemoveAsync(string dotnetVersion, DotnetComponent component)
    {
        var paths = GetComponentPaths(dotnetVersion, component);

        if (paths.Count == 0)
        {
            _logger.LogInformation("No {Component} installations match {Version}",
                component, dotnetVersion);
            return;
        }

        _logger.LogInformation("Removing {Count} {Component} installation(s) matching {Version}",
            paths.Count, component, dotnetVersion);

        foreach (var path in paths)
        {
            _logger.LogInformation("Deleting installation directory {Path}", path);
            Directory.Delete(path, true);
        }

        _logger.LogInformation("Removed {Component} installations matching {Version}",
            component, dotnetVersion);
    }

    private static VersionRange ParseVersionRange(string value)
    {
        if (!NuGetVersion.TryParse(value, out var version))
            throw new ArgumentException($"Invalid .NET version: {value}");

        var components = value.Split('-', 2)[0].Split('.').Length;

        return components switch
        {
            1 => CreateRange(new NuGetVersion(version.Major, 0, 0, "0"),
                new NuGetVersion(version.Major + 1, 0, 0, "0")),

            2 => CreateRange(new NuGetVersion(version.Major, version.Minor, 0, "0"),
                new NuGetVersion(version.Major, version.Minor + 1, 0, "0")),

            _ => new VersionRange(version, true, version, true)
        };
    }

    private static VersionRange CreateRange(NuGetVersion min, NuGetVersion max)
    {
        return new VersionRange(min, true, max, false);
    }


    private IReadOnlyCollection<string> GetComponentPaths(string dotnetVersion, DotnetComponent component)
    {
        var version = ParseVersionRange(dotnetVersion);

        return component switch
        {
            DotnetComponent.Sdk =>
                _sdkLocator.Find()
                    .Where(x => version.Satisfies(x.Version))
                    .Select(x => x.Path)
                    .ToList(),
            DotnetComponent.Runtime =>
                _runtimeLocator.Find()
                    .Where(x => x.Framework == "Microsoft.NETCore.App")
                    .Where(x => version.Satisfies(x.Version))
                    .Select(x => x.Path)
                    .ToList(),
            DotnetComponent.AspNetRuntime =>
                _runtimeLocator.Find()
                    .Where(x => x.Framework == "Microsoft.AspNetCore.App")
                    .Where(x => version.Satisfies(x.Version))
                    .Select(x => x.Path)
                    .ToList(),
            DotnetComponent.Host =>
                _hostLocator.Find()
                    .Where(x => version.Satisfies(x.Version))
                    .Select(x => x.Path)
                    .ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(component), component, null)
        };
    }
}