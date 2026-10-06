using System.Text;
using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.UserConfigurations;

internal class OhMyZshUEConfig : UEConfiguratorBase
{
    public override UEKind Kind => UEKind.OhMyZsh;
    protected override string FileName => $"60-{ApplicationInfo.Name}.zsh";

    protected override string BuildContent(string path)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"export DOTNET_ROOT=\"{path}\"");
        builder.AppendLine($"export PATH=\"{path}:$PATH\"");
        builder.AppendLine($"export PATH=\"{GetToolsPath()}:$PATH\"");

        return builder.ToString();
    }
    /*
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

        if (Directory.Exists(ohMyZsh))
            return Path.Combine(ohMyZsh, "custom");

        return null;
    }
    */
}