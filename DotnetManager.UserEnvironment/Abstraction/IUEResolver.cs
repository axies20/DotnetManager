using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Abstraction;

internal interface IUEResolver
{
    IReadOnlyCollection<UEResolvedConfiguration> Resolve();
}