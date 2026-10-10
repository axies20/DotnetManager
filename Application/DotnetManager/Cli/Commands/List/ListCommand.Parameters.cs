using System.CommandLine;

namespace DotnetManager.Cli.Commands.List;

internal sealed partial class ListCommand
{
    private readonly Option<bool> _runtimeOption = new("--runtime", "-rt")
    {
        Description = "Show installed .NET runtimes"
    };

    private readonly Option<bool> _sdkOption = new("--sdk")
    {
        Description = "Show installed .NET SDKs"
    };
}
