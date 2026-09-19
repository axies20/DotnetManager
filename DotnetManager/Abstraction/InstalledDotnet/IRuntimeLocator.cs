using DotnetManager.Models;

namespace DotnetManager.Abstraction.InstalledDotnet;

public interface IRuntimeLocator
{
    IEnumerable<RuntimeInstallation> GetAll();
}