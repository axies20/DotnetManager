using System.Runtime.InteropServices;
using DotnetManager.Core.Models;
using DotnetManager.Installation.Models.Installation.Requests;
using DotnetManager.Installation.Models.Installation.Targets;
using Spectre.Console;

namespace DotnetManager.Cli.Commands.Install;

internal sealed partial class InstallCommand
{
    private static void PrintInstallRequest(InstallRequest request)
    {
        var components = string.Join(", ", request.Options.Components.Select(GetComponentName));
        var target = request.Target switch
        {
            VersionSelector selector => $"release {selector.Version}",
            LatestSelector => "the latest eligible release",
            _ => throw new ArgumentOutOfRangeException(nameof(request))
        };
        var runtimeIdentifier = request.Options.RuntimeIdentifier ?? RuntimeInformation.RuntimeIdentifier;

        var message = $"Installing {components} for {target} (RID: {runtimeIdentifier})...";
        AnsiConsole.Write(new Text(message + Environment.NewLine));
    }

    private static string GetComponentName(DotnetComponent component)
    {
        return component switch
        {
            DotnetComponent.Sdk => ".NET SDK",
            DotnetComponent.Runtime => ".NET Runtime",
            DotnetComponent.AspNetRuntime => "ASP.NET Core Runtime",
            DotnetComponent.Host => ".NET Host",
            _ => throw new ArgumentOutOfRangeException(nameof(component))
        };
    }
}