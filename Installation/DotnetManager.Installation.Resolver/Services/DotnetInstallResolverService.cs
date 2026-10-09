using DotnetManager.Core.Models;
using DotnetManager.Installation.Resolver.Abstraction;
using DotnetManager.Installation.Resolver.Models.Requests;
using DotnetManager.Installation.Resolver.Models.Results;
using DotnetManager.Installation.Resolver.Models.Targets;
using DotnetManager.ReleaseMetadata.Abstractions;

namespace DotnetManager.Installation.Resolver.Services;

public class DotnetInstallResolverService : IDotnetInstallResolverService
{
    private readonly ISdkManifestProviderService _provider;

    public DotnetInstallResolverService(ISdkManifestProviderService provider)
    {
        _provider = provider;
    }

    public Task<ResolvedInstallation> ResolveAsync(InstallRequest request, CancellationToken cancellationToken)
    {
        return request.Target.Value switch
        {
            LatestSelector latest => ResolveLatestAsync(latest, request.Components, request.Rid, cancellationToken),
            VersionSelector version => ResolveVersionAsync(version, request.Components, request.Rid, cancellationToken),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

}
