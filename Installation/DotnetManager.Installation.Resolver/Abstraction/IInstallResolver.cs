using DotnetManager.Core.Models;
using DotnetManager.Installation.Resolver.Models.Results;

namespace DotnetManager.Installation.Resolver.Abstraction;

internal interface IInstallResolver<in TSelector>
{
    Task<ResolvedInstallation> ResolveAsync(
        TSelector selector,
        IEnumerable<DotnetComponent> components,
        string? rid,
        CancellationToken cancellationToken);
}
