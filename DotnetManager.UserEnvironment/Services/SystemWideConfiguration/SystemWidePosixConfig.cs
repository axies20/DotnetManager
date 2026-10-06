using System.Text;
using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.SystemWideConfiguration;

internal class SystemWidePosixConfig : UEConfiguratorBase
{

    public override UEKind Kind => UEKind.PosixSystem;
    protected override string FileName => $"{ApplicationInfo.Name}.sh";

    protected override string BuildContent(string path)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"export DOTNET_ROOT=\"{path}\"");
        builder.AppendLine($"export PATH=\"{path}:$PATH\"");
        builder.AppendLine($"export PATH=\"{GetToolsPath()}:$PATH\"");

        return builder.ToString();
    }
}