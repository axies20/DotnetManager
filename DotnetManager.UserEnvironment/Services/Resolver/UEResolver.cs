using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Resolver;

internal partial class UEResolver : IUEResolver
{
    private readonly IEnumerable<IUEConfigurator> _configurators;

    public UEResolver(IEnumerable<IUEConfigurator> configurators)
    {
        _configurators = configurators;
    }

    public IReadOnlyCollection<IUEConfigurator> Resolve(UEInstallScope scope)
    {
        return scope switch
        {
            UEInstallScope.User => ResolveUser(),
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
        };
    }

    private static string GetXdgConfigHome()
    {
        var xdgConfigHome =
            Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");

        if (!string.IsNullOrWhiteSpace(xdgConfigHome))
            return xdgConfigHome;

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        return Path.Combine(home, ".config");
    }

    private IReadOnlyCollection<IUEConfigurator> ResolveUser()
    {
        var configurators = new List<IUEConfigurator>();
        var zshDir = ResolveOhMyZshCustomDirectory();

        if (!string.IsNullOrEmpty(zshDir))
        {
            var zsh = _configurators.First(x => x.Kind.HasFlag(UEKind.OhMyZsh));
            configurators.Add(zsh);
        }

        return configurators;
    }
}