using DotnetManager.Models.Sdk;

namespace DotnetManager.Abstraction.SDK;

public interface ISdkInstaller
{
    Task InstallAsync(InstallSdkRequest request, CancellationToken cancellationToken);
}