using System.Text;
using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Configuration;

internal class FishConfig : UEConfiguratorBase
{
    public override UEKind Kind => UEKind.Fish;
    protected override string FileName => $"{ApplicationInfo.Name}.fish";

    protected override string BuildContent()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"set -gx DOTNET_ROOT \"{DotnetPaths.UserInstallRoot}\"");
        builder.AppendLine($"set -gx PATH \"{DotnetPaths.UserToolsRoot}\" $PATH");
        builder.AppendLine($"set -gx PATH \"{DotnetPaths.UserInstallRoot}\" $PATH");
        return builder.ToString();
    }
}
