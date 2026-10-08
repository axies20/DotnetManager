using DotnetManager.Core.Models;

namespace DotnetManager.Removal.Abstractions;

internal interface IDotnetRemovalTargetResolverService
{
    IReadOnlyCollection<string> Resolve(string dotnetVersion, DotnetComponent component);
}