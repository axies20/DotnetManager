using DotnetManager.InstalledDotnet.Abstractions;
using Microsoft.Win32;

namespace DotnetManager.InstalledDotnet.RootSources;

public sealed class WindowsRegistryDotnetRootSource : IDotnetRootSource
{
    private const string InstalledVersionsPath = @"SOFTWARE\dotnet\Setup\InstalledVersions";

    public IEnumerable<string> DiscoverRoots()
    {
        if (!OperatingSystem.IsWindows())
            yield break;

        using var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

        using var installedVersions = localMachine.OpenSubKey(InstalledVersionsPath);

        if (installedVersions is null)
            yield break;

        foreach (var architecture in installedVersions.GetSubKeyNames())
        {
            using var architectureKey = installedVersions.OpenSubKey(architecture);

            if (architectureKey?.GetValue("InstallLocation") is string installLocation &&
                !string.IsNullOrWhiteSpace(installLocation))
            {
                yield return installLocation;
            }
        }
    }
}