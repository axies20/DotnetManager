using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Abstraction;

public interface IUEManager
{
    Task ConfigureEnvironment();
    void RemoveEnvironment();
    void RemoveEnvironment(UEKind kind);
}