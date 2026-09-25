using DotnetManager.Installation.Models.Downloads;
using DotnetManager.Installation.Models.Installation.Requests;

namespace DotnetManager.Installation.Abstractions.Resolver;

public interface IDotnetInstallResolverService
{
    Task<IReadOnlyCollection<DotnetDownloadSource>> ResolveAsync(InstallRequest request,
        CancellationToken cancellationToken);
}
