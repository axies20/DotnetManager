using DotnetManager.Installation.Abstractions.InstallPaths;
using DotnetManager.Installation.Abstractions.Removal;
using DotnetManager.Installation.Abstractions.UserEnvironment;
using DotnetManager.Installation.Models;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;
using NuGet.Versioning;

namespace DotnetManager.Installation.Services.Removal;

public class DotnetRemovalService : IDotnetRemovalService
{
    private readonly IDotnetInstallationLocatorService<HostInstallation> _hostLocator;
    private readonly IDotnetInstallationLocatorService<RuntimeInstallation> _runtimeLocator;
    private readonly IDotnetInstallationLocatorService<SdkInstallation> _sdkLocator;
    private readonly IDotnetInstallPathProviderService _pathProvider;
    private readonly IUserEnvironmentConfiguratorService _userEnvironmentConfigurator;


    public DotnetRemovalService(IDotnetInstallationLocatorService<HostInstallation> hostLocator,
        IDotnetInstallationLocatorService<RuntimeInstallation> runtimeLocator,
        IDotnetInstallationLocatorService<SdkInstallation> sdkLocator,
        IDotnetInstallPathProviderService pathProvider,
        IUserEnvironmentConfiguratorService userEnvironmentConfigurator)
    {
        _hostLocator = hostLocator;
        _runtimeLocator = runtimeLocator;
        _sdkLocator = sdkLocator;
        _pathProvider = pathProvider;
        _userEnvironmentConfigurator = userEnvironmentConfigurator;
    }


    public async Task RemoveAsync(string dotnetVersion, bool cleanupPath, CancellationToken cancellationToken)
    {
        foreach (var component in Enum.GetValues<DotnetComponent>())
            Remove(dotnetVersion, component);

        if (cleanupPath)
            await CleanupPathAsync(cancellationToken);
    }


    public async Task RemoveAsync(string dotnetVersion,
        DotnetComponent component,
        bool cleanupPath,
        CancellationToken cancellationToken)
    {
        Remove(dotnetVersion, component);

        if (cleanupPath)
            await CleanupPathAsync(cancellationToken);
    }

    private void Remove(string dotnetVersion, DotnetComponent component)
    {
        var paths = GetComponentPaths(dotnetVersion, component);

        foreach (var path in paths)
            Directory.Delete(path, true);
    }

    private async Task CleanupPathAsync(CancellationToken cancellationToken)
    {
        var installRoot = Path.GetFullPath(_pathProvider.GetInstallDirectory());

        if (HasInstallationsInRoot(installRoot))
            return;

        var linkPath = _pathProvider.GetExecutableLinkPath();

        if (linkPath is not null)
        {
            RemoveManagedExecutableLink(linkPath, Path.Combine(installRoot, "dotnet"));
            return;
        }

        await _userEnvironmentConfigurator.RemoveConfigurationAsync(installRoot, cancellationToken);
    }

    private bool HasInstallationsInRoot(string installRoot)
    {
        if (_sdkLocator.Find().Any(x => IsPathInRoot(x.Path, installRoot)))
            return true;

        if (_runtimeLocator.Find().Any(x => IsPathInRoot(x.Path, installRoot)))
            return true;

        return _hostLocator.Find().Any(x => IsPathInRoot(x.Path, installRoot));
    }

    private static bool IsPathInRoot(string path, string installRoot)
    {
        var relativePath = Path.GetRelativePath(installRoot, Path.GetFullPath(path));

        if (relativePath == "..")
            return false;

        if (relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            return false;

        return !Path.IsPathRooted(relativePath);
    }

    private static void RemoveManagedExecutableLink(string linkPath, string expectedTargetPath)
    {
        var link = new FileInfo(linkPath);

        if (link.LinkTarget is null)
            return;

        var linkDirectory = link.DirectoryName;

        if (linkDirectory == null)
        {
            throw new InvalidOperationException(
                $"Unable to determine the directory containing symbolic link '{linkPath}'.");
        }

        string targetPath;
        if (Path.IsPathRooted(link.LinkTarget))
            targetPath = Path.GetFullPath(link.LinkTarget);
        else
            targetPath = Path.GetFullPath(link.LinkTarget, linkDirectory);

        StringComparison comparison;
        if (OperatingSystem.IsWindows())
            comparison = StringComparison.OrdinalIgnoreCase;
        else
            comparison = StringComparison.Ordinal;

        if (string.Equals(targetPath, Path.GetFullPath(expectedTargetPath), comparison))
            File.Delete(linkPath);
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

    private static VersionRange ParseVersionRange(string value)
    {
        if (!NuGetVersion.TryParse(value, out var version))
            throw new ArgumentException($"Invalid .NET version: {value}");

        var components = value.Split('-', 2)[0].Split('.').Length;

        return components switch
        {
            1 => new VersionRange(new NuGetVersion(version.Major, 0, 0), true,
                new NuGetVersion(version.Major + 1, 0, 0)),

            2 => new VersionRange(new NuGetVersion(version.Major, version.Minor, 0), true,
                new NuGetVersion(version.Major, version.Minor + 1, 0)),

            _ => new VersionRange(version, true, version, true)
        };
    }

}