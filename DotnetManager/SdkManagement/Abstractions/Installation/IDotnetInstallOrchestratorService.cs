using DotnetManager.SdkManagement.Models.Installation.Requests;

namespace DotnetManager.SdkManagement.Abstractions.Installation;

public interface IDotnetInstallOrchestratorService
{
    Task InstallAsync(InstallRequest request, CancellationToken cancellationToken);
}
