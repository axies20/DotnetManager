using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Resolver;

internal partial class UEResolver
{

    private static string? ResolveOhMyZshCustomDirectory()
    {
        var zshCustom = Environment.GetEnvironmentVariable("ZSH_CUSTOM");

        if (!string.IsNullOrWhiteSpace(zshCustom))
            return zshCustom;

        var zsh = Environment.GetEnvironmentVariable("ZSH");

        if (!string.IsNullOrWhiteSpace(zsh))
            return Path.Combine(zsh, "custom");

        var ohMyZsh = Path.Combine(UserPaths.Home, ".oh-my-zsh");

        return Directory.Exists(ohMyZsh) ? Path.Combine(ohMyZsh, "custom") : null;
    }

    private UEResolvedConfiguration? ResolveZsh()
    {
        var zshDir = ResolveOhMyZshCustomDirectory();

        if (string.IsNullOrEmpty(zshDir))
        {
            return null;
        }

        var zsh = configurators.First(x => x.Kind == UEKind.OhMyZsh);
        return new UEResolvedConfiguration(zsh, zshDir);

    }
}