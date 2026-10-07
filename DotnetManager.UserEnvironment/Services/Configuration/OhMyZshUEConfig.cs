using System.Text;
using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Configuration;

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
}