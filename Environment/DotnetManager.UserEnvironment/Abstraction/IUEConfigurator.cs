using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Abstraction;

internal interface IUEConfigurator
{
    UEKind Kind { get; }
    Task ConfigureAsync(string path, CancellationToken cancellationToken);
    void Remove(string path);
}