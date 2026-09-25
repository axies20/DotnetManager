using DotnetManager.Installation.Models;

namespace DotnetManager.Installation.Abstractions.Removal;

public interface IDotnetRemovalService
{
    void RemoveAsync(string dotnetVersion);
    void RemoveAsync(string dotnetVersion, DotnetComponent component);
}