using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;

namespace DotnetManager.Cli.Commands.List;

public sealed partial class ListCommand : ICommand
{
    private readonly IDotnetInstallationLocatorService<HostInstallation> _hostLocator;
    private readonly IDotnetInstallationLocatorService<RuntimeInstallation> _runtimeLocator;
    private readonly IDotnetInstallationLocatorService<SdkInstallation> _sdkLocator;

    public ListCommand(IDotnetInstallationLocatorService<SdkInstallation> sdkLocator,
        IDotnetInstallationLocatorService<RuntimeInstallation> runtimeLocator,
        IDotnetInstallationLocatorService<HostInstallation> hostLocator)
    {
        _sdkLocator = sdkLocator;
        _runtimeLocator = runtimeLocator;
        _hostLocator = hostLocator;
    }

    public Command Initialize()
    {
        var command = new Command("list", "Show installed .NET SDKs, runtimes, and hosts.");

        command.Options.Add(_sdkOption);
        command.Options.Add(_runtimeOption);
        command.Options.Add(_hostOption);

        command.SetAction(Execute);

        return command;
    }
}
