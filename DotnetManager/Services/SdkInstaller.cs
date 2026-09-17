using DotnetManager.Abstraction.SDK;
using DotnetManager.Models.Sdk;

namespace DotnetManager.Services;

public class SdkInstaller : ISdkInstaller
{
    public Task InstallAsync(InstallSdkRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}