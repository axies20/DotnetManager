using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Resolver;

internal partial class UEResolver(IEnumerable<IUEConfigurator> configurators) : IUEResolver
{
    public IReadOnlyCollection<UEResolvedConfiguration> Resolve()
    {
        var resolve = new List<UEResolvedConfiguration>();

        var zsh = ResolveZsh();
        if (zsh != null)
            resolve.Add(zsh);

        var fish = ResolveFish();
        if (fish != null)
            resolve.Add(fish);

        var envD = ResolveEnvironmentD();
        if (envD != null)
            resolve.Add(envD);

        return resolve;
    }
}