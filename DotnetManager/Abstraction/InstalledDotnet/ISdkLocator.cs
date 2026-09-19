using DotnetManager.Models;

namespace DotnetManager.Abstraction.InstalledDotnet;

public interface ISdkLocator
{
    IEnumerable<SdkInstallation> GetAll();
}