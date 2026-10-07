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

        var home = Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile);

        var ohMyZsh = Path.Combine(home, ".oh-my-zsh");

        return Directory.Exists(ohMyZsh) ? Path.Combine(ohMyZsh, "custom") : null;
    }
}