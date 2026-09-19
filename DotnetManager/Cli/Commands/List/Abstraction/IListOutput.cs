using DotnetManager.InstalledDotnet.Models;

namespace DotnetManager.Cli.Commands.List.Abstraction;

public interface IListOutput
{
    void PrintSdks(IReadOnlyCollection<SdkInstallation> sdks);
    void PrintRuntimes(IReadOnlyCollection<RuntimeInstallation> runtimes);
    void PrintHosts(IReadOnlyCollection<HostInstallation> hosts);
}