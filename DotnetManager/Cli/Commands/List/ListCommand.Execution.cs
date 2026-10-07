using System.CommandLine;

namespace DotnetManager.Cli.Commands.List;

internal sealed partial class ListCommand
{
    private void Execute(ParseResult result)
    {
        var showSdk = result.GetValue(_sdkOption);
        var showRuntime = result.GetValue(_runtimeOption);
        var showHost = result.GetValue(_hostOption);

        var showAll = !showSdk && !showRuntime && !showHost;

        if (showAll || showSdk)
            PrintSdks(_sdkLocator.Find());

        if (showAll || showRuntime)
            PrintRuntimes(_runtimeLocator.Find());

        if (showAll || showHost)
            PrintHosts(_hostLocator.Find());
    }
}