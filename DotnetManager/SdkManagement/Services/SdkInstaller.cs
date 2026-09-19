using DotnetManager.SdkManagement.Abstractions;
using DotnetManager.SdkManagement.Models;

namespace DotnetManager.SdkManagement.Services;

public class SdkInstaller : ISdkInstaller
{
    public Task InstallAsync(InstallSdkRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}