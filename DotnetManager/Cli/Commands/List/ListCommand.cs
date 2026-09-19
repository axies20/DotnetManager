using System.CommandLine;
using DotnetManager.Cli.Commands.List.Abstraction;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;

namespace DotnetManager.Cli.Commands;

public sealed class ListCommand
{
    private readonly IDotnetInstallationLocator<SdkInstallation> _sdkLocator;
    private readonly IDotnetInstallationLocator<RuntimeInstallation> _runtimeLocator;
    private readonly IDotnetInstallationLocator<HostInstallation> _hostLocator;
    private readonly IListOutput _output;

    private readonly Option<bool> _sdkOption = new("--sdk")
    {
        Description = "Show installed .NET SDKs"
    };

    private readonly Option<bool> _runtimeOption = new("--runtime")
    {
        Description = "Show installed .NET runtimes"
    };

    private readonly Option<bool> _hostOption = new("--host")
    {
        Description = "Show installed .NET hosts"
    };

    public ListCommand(
        IDotnetInstallationLocator<SdkInstallation> sdkLocator,
        IDotnetInstallationLocator<RuntimeInstallation> runtimeLocator,
        IDotnetInstallationLocator<HostInstallation> hostLocator,
        IListOutput output)
    {
        _sdkLocator = sdkLocator;
        _runtimeLocator = runtimeLocator;
        _hostLocator = hostLocator;
        _output = output;
    }

    public Command Initialize()
    {
        var command = new Command(
            "list",
            "Show installed .NET SDKs, runtimes, and hosts.");

        command.Options.Add(_sdkOption);
        command.Options.Add(_runtimeOption);
        command.Options.Add(_hostOption);

        command.SetAction(Execute);

        return command;
    }

    private void Execute(ParseResult result)
    {
        var showSdk = result.GetValue(_sdkOption);
        var showRuntime = result.GetValue(_runtimeOption);
        var showHost = result.GetValue(_hostOption);

        var showAll = !showSdk && !showRuntime && !showHost;

        if (showAll || showSdk)
            _output.PrintSdks(_sdkLocator.Find());

        if (showAll || showRuntime)
            _output.PrintRuntimes(_runtimeLocator.Find());

        if (showAll || showHost)
            _output.PrintHosts(_hostLocator.Find());
    }
}