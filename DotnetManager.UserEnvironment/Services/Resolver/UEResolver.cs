using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Resolver;

internal partial class UEResolver(IEnumerable<IUEConfigurator> configurators) : IUEResolver
{
    public IReadOnlyCollection<UEResolvedConfiguration> Resolve()
    {
        var configurators = new List<UEResolvedConfiguration>();

        var zsh = ResolveZsh();
        if (zsh != null)
            configurators.Add(zsh);

        var fish = ResolveFish();
        if (fish != null)
            configurators.Add(fish);

        var envD = ResolveEnvironmentD();
        if (envD != null)
            configurators.Add(envD);

        return configurators;
    }

    private static string GetXdgConfigHome()
    {
        var xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");

        if (!string.IsNullOrWhiteSpace(xdgConfigHome))
            return xdgConfigHome;

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        return Path.Combine(home, ".config");
    }
}