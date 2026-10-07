using DotnetManager.UserEnvironment.Helper;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Resolver;

internal partial class UEResolver
{
    private UEResolvedConfiguration? ResolveEnvironmentD()
    {
        if (!SystemdDetector.IsSystemd())
        {
            return null;
        }

        var path = Path.Combine(GetXdgConfigHome(), "environment.d");
        var envD = configurators.First(x => x.Kind == UEKind.EnvironmentD);
        return new UEResolvedConfiguration(envD, path);
    }
}