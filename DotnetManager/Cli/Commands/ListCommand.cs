using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Cli.Output;
using DotnetManager.InstalledDotnet.Abstractions;
using DotnetManager.InstalledDotnet.Models;

namespace DotnetManager.Cli.Commands;

public sealed class ListCommand : ICommand
{
    private readonly IDotnetInstallationLocatorService<HostInstallation> _hostLocator;
    private readonly IDotnetInstallationLocatorService<RuntimeInstallation> _runtimeLocator;
    private readonly IDotnetInstallationLocatorService<SdkInstallation> _sdkLocator;

    private readonly Option<bool> _hostOption = new("--host", "-ht")
    {
        Description = "Show installed .NET hosts"
    };

    private readonly Option<bool> _runtimeOption = new("--runtime", "-rt")
    {
        Description = "Show installed .NET runtimes"
    };

    private readonly Option<bool> _sdkOption = new("--sdk")
    {
        Description = "Show installed .NET SDKs"
    };

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

    private static void PrintSdks(IReadOnlyCollection<SdkInstallation> sdks)
    {
        TableColumn<SdkInstallation>[] columns =
        [
            new("Version", x => x.Version.ToString()),
            new("Path", x => x.Path)
        ];

        TablePrinter.Print("Installed SDKs", sdks, columns);
    }

    private static void PrintRuntimes(IReadOnlyCollection<RuntimeInstallation> runtimes)
    {
        TableColumn<RuntimeInstallation>[] columns =
        [
            new("Framework", x => x.Framework),
            new("Version", x => x.Version.ToString()),
            new("Path", x => x.Path)
        ];

        TablePrinter.Print("Installed Runtimes", runtimes, columns);
    }

    private static void PrintHosts(IReadOnlyCollection<HostInstallation> hosts)
    {
        TableColumn<HostInstallation>[] columns =
        [
            new("Version", x => x.Version.ToString()),
            new("Path", x => x.Path)
        ];

        TablePrinter.Print("Installed Hosts", hosts, columns);
    }

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
