using DotnetManager.Installation.Resolver.Abstraction;
using DotnetManager.Installation.Resolver.Models.Requests;
using DotnetManager.Installation.Resolver.Models.Results;
using DotnetManager.Installation.Resolver.Models.Targets;

namespace DotnetManager.Installation.Resolver.Services;

internal sealed class DotnetInstallResolverService : IDotnetInstallResolverService
{
    private readonly IInstallResolver<LatestSelector> _latestResolver;
    private readonly IInstallResolver<VersionSelector> _versionResolver;

    public DotnetInstallResolverService(
        IInstallResolver<LatestSelector> latestResolver,
        IInstallResolver<VersionSelector> versionResolver)
    {
        _latestResolver = latestResolver;
        _versionResolver = versionResolver;
    }

    public Task<ResolvedInstallation> ResolveAsync(InstallRequest request, CancellationToken cancellationToken)
    {
        return request.Target.Value switch
        {
            LatestSelector latest => _latestResolver.ResolveAsync(latest, request.Components, request.Rid,
                cancellationToken),
            VersionSelector version => _versionResolver.ResolveAsync(version, request.Components, request.Rid,
                cancellationToken),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}