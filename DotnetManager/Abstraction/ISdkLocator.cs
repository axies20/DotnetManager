using DotnetManager.Models;

namespace DotnetManager.Abstraction;

public interface ISdkLocator
{
    IReadOnlyCollection<SdkInstallation> GetInstalled();
}