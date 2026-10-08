using System.Text;
using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;
using Microsoft.Extensions.Logging;

namespace DotnetManager.UserEnvironment.Services.Configuration;

internal sealed class EnvironmentDUEConfig(ILogger<EnvironmentDUEConfig> logger) : UEConfiguratorBase(logger)
{
    public override UEKind Kind => UEKind.EnvironmentD;
    protected override string FileName => $"60-{ApplicationInfo.Name}.conf";

    protected override string BuildContent()
    {
        var builder = new StringBuilder();

        builder.AppendLine($"DOTNET_ROOT=\"{DotnetPaths.UserInstallRoot}\"");
        builder.AppendLine($"PATH=\"{DotnetPaths.UserToolsRoot}:$PATH\"");
        builder.AppendLine($"PATH=\"{DotnetPaths.UserInstallRoot}:$PATH\"");

        return builder.ToString();
    }
}