using System.Text;
using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.UserConfigurations;

internal class SystemdUEConfig : UEConfiguratorBase
{
    public override UEKind Kind => UEKind.SystemdUser;
    protected override string FileName => $"60-{ApplicationInfo.Name}.conf";

    protected override string BuildContent(string path)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"DOTNET_ROOT=\"{path}\"");
        builder.AppendLine($"PATH=\"{GetToolsPath()}:$PATH\"");
        builder.AppendLine($"PATH=\"{path}:$PATH\"");

        return builder.ToString();
    }
}