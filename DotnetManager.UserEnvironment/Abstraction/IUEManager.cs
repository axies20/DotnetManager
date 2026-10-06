using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Abstraction;

public interface IUEManager
{
    Task ConfigureEnvironment(UEInstallScope scope);
}