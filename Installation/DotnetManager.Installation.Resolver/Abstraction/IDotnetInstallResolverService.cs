using DotnetManager.Installation.Resolver.Models.Requests;
using DotnetManager.Installation.Resolver.Models.Results;

namespace DotnetManager.Installation.Resolver.Abstraction;

internal interface IDotnetInstallResolverService
{
    Task<ResolvedInstallation> ResolveAsync(InstallRequest request, CancellationToken cancellationToken);
}