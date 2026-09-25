using DotnetManager.Installation.Models.Installation.Requests;

namespace DotnetManager.Installation.Abstractions.Installation;

public interface IDotnetInstallOrchestratorService
{
    Task InstallAsync(InstallRequest request, CancellationToken cancellationToken);
}
