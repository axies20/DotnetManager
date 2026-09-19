using DotnetManager.InstalledDotnet.Abstractions;

namespace DotnetManager.InstalledDotnet.RootSources;

public class UnixInstallLocationDotnetRootSource : IDotnetRootSource
{
    private const string DotnetConfigDirectory = "/etc/dotnet";

    public IEnumerable<string> DiscoverRoots()
    {
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
        {
            yield break;
        }
        
        if (!Directory.Exists(DotnetConfigDirectory))
            yield break;

        foreach (var file in Directory.EnumerateFiles(DotnetConfigDirectory, "install_location*"))
        {
            var root = File.ReadLines(file)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(root))
                yield return root.Trim();
        }
    }
}