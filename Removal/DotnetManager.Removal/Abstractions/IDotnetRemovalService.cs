using DotnetManager.Core.Models;


namespace DotnetManager.Removal.Abstractions;

public interface IDotnetRemovalService
{
    void RemoveAsync(string dotnetVersion);
    void RemoveAsync(string dotnetVersion, DotnetComponent component);
}