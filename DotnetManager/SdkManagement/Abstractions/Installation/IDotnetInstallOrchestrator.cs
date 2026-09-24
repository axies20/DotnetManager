using DotnetManager.SdkManagement.Models.Installation.Requests;

namespace DotnetManager.SdkManagement.Abstractions.Installation;

public interface IDotnetInstallOrchestrator
{
    Task InstallAsync(InstallRequest request, CancellationToken cancellationToken);
}