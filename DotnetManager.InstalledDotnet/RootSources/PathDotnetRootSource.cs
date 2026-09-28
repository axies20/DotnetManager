using DotnetManager.InstalledDotnet.Abstractions;

namespace DotnetManager.InstalledDotnet.RootSources;

public class PathDotnetRootSource : IDotnetRootSource
{
    private const string PathVariable = "PATH";

    public IEnumerable<string> DiscoverRoots()
    {
        var path = Environment.GetEnvironmentVariable(PathVariable);

        if (path != null)
            return GetRootsFromPath(path);

        return [];
    }

    private static IEnumerable<string> GetRootsFromPath(string path)
    {
        var directories = path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        var executableName = OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet";

        foreach (var directory in directories)
        {
            var executablePath = Path.Combine(directory, executableName);

            if (!File.Exists(executablePath))
                continue;

            var target = File.ResolveLinkTarget(executablePath, true);

            var actualPath = target?.FullName ?? executablePath;
            var root = Path.GetDirectoryName(actualPath);

            if (root is not null)
                yield return root;
        }
    }
}