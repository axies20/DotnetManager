using DotnetManager.Installation.Models;

namespace DotnetManager.Installation.Abstractions.Removal;

public interface IDotnetRemovalService
{
    Task RemoveAsync(string dotnetVersion, bool cleanupPath, CancellationToken cancellationToken);

    Task RemoveAsync(string dotnetVersion,
        DotnetComponent component,
        bool cleanupPath,
        CancellationToken cancellationToken);
}