using DotnetManager.UserEnvironment.Helper;

namespace DotnetManager.UserEnvironment.Services.Resolver;

internal partial class UEResolver
{
    private static string? ResolveEnvironmentD()
    {
        if (!SystemdDetector.IsSystemd())
        {
            return null;
        }

        var path = Path.Combine(GetXdgConfigHome(), "environment.d");
        return path;
    }
}