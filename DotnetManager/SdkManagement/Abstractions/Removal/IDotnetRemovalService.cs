using DotnetManager.SdkManagement.Models;

namespace DotnetManager.SdkManagement.Abstractions.Removal;

public interface IDotnetRemovalService
{
    void RemoveAsync(string dotnetVersion);
    void RemoveAsync(string dotnetVersion, DotnetComponent component);
}