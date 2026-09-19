using DotnetManager.Models;

namespace DotnetManager.Abstraction.InstalledDotnet;

public interface IHostInstallation
{
    IEnumerable<HostInstallation> GetAll();

}