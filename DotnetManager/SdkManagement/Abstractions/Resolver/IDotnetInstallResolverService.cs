using DotnetManager.SdkManagement.Models.Downloads;
using DotnetManager.SdkManagement.Models.Installation.Requests;

namespace DotnetManager.SdkManagement.Abstractions.Resolver;

public interface IDotnetInstallResolverService
{
    Task<IReadOnlyCollection<DotnetDownloadSource>> ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken);
}
