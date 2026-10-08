using DotnetManager.Core.Models;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using DotnetManager.Removal.Abstractions;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace DotnetManager.Removal.Services;

internal sealed class DotnetRemovalTargetResolver : IDotnetRemovalTargetResolverService
{
    private readonly IDotnetInstallationLocatorService<HostInstallation> _hostLocator;
    private readonly IDotnetInstallationLocatorService<RuntimeInstallation> _runtimeLocator;
    private readonly IDotnetInstallationLocatorService<SdkInstallation> _sdkLocator;
    private readonly ILogger<DotnetRemovalTargetResolver> _logger;

    public DotnetRemovalTargetResolver(
        IDotnetInstallationLocatorService<HostInstallation> hostLocator,
        IDotnetInstallationLocatorService<RuntimeInstallation> runtimeLocator,
        IDotnetInstallationLocatorService<SdkInstallation> sdkLocator,
        ILogger<DotnetRemovalTargetResolver> logger)
    {
        _hostLocator = hostLocator;
        _runtimeLocator = runtimeLocator;
        _sdkLocator = sdkLocator;
        _logger = logger;
    }

    public IReadOnlyCollection<string> Resolve(string dotnetVersion, DotnetComponent component)
    {
        _logger.LogDebug("Resolving removal targets for {Component} matching {Version}",
            component, dotnetVersion);

        var version = ParseVersionRange(dotnetVersion);

        var paths = component switch
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

        _logger.LogDebug("Resolved {TargetCount} removal target(s) for {Component} matching {Version}",
            paths.Count, component, dotnetVersion);

        return paths;
    }

    private static VersionRange ParseVersionRange(string value)
    {
        if (!NuGetVersion.TryParse(value, out var version))
            throw new ArgumentException($"Invalid .NET version: {value}");

        var components = value.Split('-', 2);
        var length = components.First().Split('.').Length;

        switch (length)
        {
            case 1:
            {
                var minVersion = new NuGetVersion(version.Major, 0, 0, "0");
                var maxVersion = new NuGetVersion(version.Major + 1, 0, 0, "0");
                return CreateRange(minVersion, maxVersion);
            }
            case 2:
            {
                var minVersion = new NuGetVersion(version.Major, version.Minor, 0, "0");
                var maxVersion = new NuGetVersion(version.Major, version.Minor + 1, 0, "0");
                return CreateRange(minVersion, maxVersion);
            }
            default:
                return new VersionRange(version, true, version, true);
        }
    }

    private static VersionRange CreateRange(NuGetVersion min, NuGetVersion max)
    {
        return new VersionRange(min, true, max, false);
    }
}