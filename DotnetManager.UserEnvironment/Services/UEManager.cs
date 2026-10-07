using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services;

internal class UEManager(IUEResolver resolver) : IUEManager
{
    public async Task ConfigureEnvironment()
    {
        foreach (var resolvedConfiguration in resolver.Resolve())
        {
            await resolvedConfiguration.Configurator.ConfigureAsync(resolvedConfiguration.Path,
                CancellationToken.None);
        }
    }

    public void RemoveEnvironment()
    {
        foreach (var resolvedConfiguration in resolver.Resolve())
        {
            resolvedConfiguration.Configurator.Remove(resolvedConfiguration.Path);
        }
    }

    public void RemoveEnvironment(UEKind kind)
    {
        var config = resolver.Resolve().FirstOrDefault(x => x.Configurator.Kind == kind);
        config?.Configurator.Remove(config.Path);
    }
}