using DotnetManager.SdkManagement.Models;

namespace DotnetManager.SdkManagement.Abstractions;

public interface ISdkInstaller
{
    Task InstallAsync(InstallSdkRequest request, CancellationToken cancellationToken);
}