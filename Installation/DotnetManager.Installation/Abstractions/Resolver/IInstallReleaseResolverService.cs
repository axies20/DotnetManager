using DotnetManager.Installation.Models.Installation.Targets;
using DotnetManager.ReleaseMetadata.Models.Index;
using DotnetManager.ReleaseMetadata.Models.Releases;

namespace DotnetManager.Installation.Abstractions.Resolver;

public interface IInstallReleaseResolverService<in TSelector> where TSelector : InstallTarget
{
    Task<SdkRelease> ResolveAsync(IEnumerable<SdkChannel> channels,
        TSelector selector,
        CancellationToken cancellationToken);
}