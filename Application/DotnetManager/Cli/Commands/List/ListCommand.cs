using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;

namespace DotnetManager.Cli.Commands.List;

internal sealed partial class ListCommand : ICommand
{
    private readonly IDotnetInstallationLocatorService<RuntimeInstallation> _runtimeLocator;
    private readonly IDotnetInstallationLocatorService<SdkInstallation> _sdkLocator;

    public ListCommand(IDotnetInstallationLocatorService<SdkInstallation> sdkLocator,
        IDotnetInstallationLocatorService<RuntimeInstallation> runtimeLocator)
    {
        _sdkLocator = sdkLocator;
        _runtimeLocator = runtimeLocator;
    }

    public Command Initialize()
    {
        var command = new Command("list", "Show installed .NET SDKs and runtimes.");

        command.Options.Add(_sdkOption);
        command.Options.Add(_runtimeOption);

        command.SetAction(Execute);

        return command;
    }
}
