using DotnetManager.SdkManagement.Models.Downloads;
using DotnetManager.SdkManagement.Models.Installation.Requests;

namespace DotnetManager.SdkManagement.Abstractions.Resolver;

public interface IDotnetInstallResolver
{
    Task<IReadOnlyCollection<DotnetDownloadSource>> ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken);
}
