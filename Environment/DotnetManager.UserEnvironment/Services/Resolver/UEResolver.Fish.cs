using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Resolver;

internal partial class UEResolver
{

    private static string? ResolveFishConfigDirectory()
    {
        if (!IsFishInstalled())
        {
            return null;
        }

        return Path.Combine(UserPaths.XdgConfigHome, "fish", "conf.d");
    }

    private static bool IsFishInstalled()
    {
        var path = Environment.GetEnvironmentVariable("PATH");

        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        foreach (var directory in path.Split(Path.PathSeparator))
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                continue;
            }

            var fish = Path.Combine(directory, "fish");

            if (File.Exists(fish))
            {
                return true;
            }
        }

        return false;
    }

    private UEResolvedConfiguration? ResolveFish()
    {
        var fishDir = ResolveFishConfigDirectory();

        if (string.IsNullOrWhiteSpace(fishDir))
        {
            return null;
        }

        var fish = configurators.First(x => x.Kind == UEKind.Fish);
        return new UEResolvedConfiguration(fish, fishDir);
    }
}