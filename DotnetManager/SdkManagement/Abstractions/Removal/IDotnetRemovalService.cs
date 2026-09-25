using DotnetManager.SdkManagement.Models;
using NuGet.Versioning;

namespace DotnetManager.SdkManagement.Abstractions.Removal;

public interface IDotnetRemovalService
{
    void RemoveAsync(string dotnetVersion);
    void RemoveAsync(string dotnetVersion, DotnetComponent component);
}