using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Abstraction;

internal interface IUEManager
{
    Task ConfigureEnvironment();
}
