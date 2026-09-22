using DotnetManager.SdkManagement.Models.Installation;

namespace DotnetManager.SdkManagement.Abstractions.Resolver;

public interface IDotnetInstallResolver
{
    Task ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken = default);
}
