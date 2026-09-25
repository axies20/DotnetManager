using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using DotnetManager.SdkManagement.Abstractions.Removal;
using DotnetManager.SdkManagement.Models;
using NuGet.Versioning;

namespace DotnetManager.SdkManagement.Services.Removal;

public class DotnetRemovalService : IDotnetRemovalService
{
    private readonly IDotnetInstallationLocatorService<HostInstallation> _hostLocator;
    private readonly IDotnetInstallationLocatorService<RuntimeInstallation> _runtimeLocator;
    private readonly IDotnetInstallationLocatorService<SdkInstallation> _sdkLocator;


    public DotnetRemovalService(IDotnetInstallationLocatorService<HostInstallation> hostLocator,
        IDotnetInstallationLocatorService<RuntimeInstallation> runtimeLocator,
        IDotnetInstallationLocatorService<SdkInstallation> sdkLocator)
    {
        _hostLocator = hostLocator;
        _runtimeLocator = runtimeLocator;
        _sdkLocator = sdkLocator;
    }


    public void RemoveAsync(string dotnetVersion)
    {
        foreach (var component in Enum.GetValues<DotnetComponent>())
        {
            RemoveAsync(dotnetVersion, component);
        }
    }


    public void RemoveAsync(string dotnetVersion, DotnetComponent component)
    {
        var paths = GetComponentPaths(dotnetVersion, component);

        foreach (var path in paths)
        {
            Directory.Delete(path, true);
        }
    }


    private IReadOnlyCollection<string> GetComponentPaths(string dotnetVersion, DotnetComponent component)
    {
        if (!VersionRange.TryParse(dotnetVersion, out var version))
        {
            throw new ArgumentException("Invalid dotnet version");
        }

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