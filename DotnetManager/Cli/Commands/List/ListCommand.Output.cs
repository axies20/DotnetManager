using DotnetManager.Cli.Output;
using DotnetManager.InstalledDotnet.Models;

namespace DotnetManager.Cli.Commands.List;

internal sealed partial class ListCommand
{
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
}
