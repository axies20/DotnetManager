using System.Text;
using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services;

internal class FishConfig : UEConfiguratorBase
{
    public override UEKind Kind => UEKind.FishUser | UEKind.FishSystem;
    protected override string FileName => $"{ApplicationInfo.Name}.fish";

    protected override string BuildContent(string dotnetRoot)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"set -gx DOTNET_ROOT \"{dotnetRoot}\"");
        builder.AppendLine($"set -gx PATH \"{GetToolsPath()}\" $PATH");
        builder.AppendLine($"set -gx PATH \"{dotnetRoot}\" $PATH");
        return builder.ToString();
    }
}