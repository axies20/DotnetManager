using System.Text;
using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Configuration;

internal class OhMyZshUEConfig : UEConfiguratorBase
{
    public override UEKind Kind => UEKind.OhMyZsh;
    protected override string FileName => $"60-{ApplicationInfo.Name}.zsh";

    protected override string BuildContent()
    {
        var builder = new StringBuilder();

        builder.AppendLine($"export DOTNET_ROOT=\"{DotnetPaths.UserInstallRoot}\"");
        builder.AppendLine($"export PATH=\"{DotnetPaths.UserInstallRoot}:$PATH\"");
        builder.AppendLine($"export PATH=\"{DotnetPaths.UserToolsRoot}:$PATH\"");

        return builder.ToString();
    }
}