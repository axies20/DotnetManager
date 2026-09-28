using System.CommandLine;
using DotnetManager.Core.Models;

namespace DotnetManager.Cli.Commands.Remove;

public partial class RemoveCommand
{
    private void Execute(ParseResult result)
    {
        var components = GetComponentsToRemove(result);
        var version = result.GetValue(_version);

        if (string.IsNullOrEmpty(version))
        {
            throw new ArgumentException("No components or version specified.");
        }

        if (components.Count == 0)
        {
            _removalService.RemoveAsync(version);
        }

        foreach (var component in components)
        {
            _removalService.RemoveAsync(version, component);
        }
    }

    private IReadOnlyCollection<DotnetComponent> GetComponentsToRemove(ParseResult result)
    {
        List<DotnetComponent> components = [];

        if (result.GetValue(_runtime))
            components.Add(DotnetComponent.Runtime);

        if (result.GetValue(_asp))
            components.Add(DotnetComponent.AspNetRuntime);

        if (result.GetValue(_host))
            components.Add(DotnetComponent.Host);

        if (result.GetValue(_sdk))
            components.Add(DotnetComponent.Sdk);

        return components;
    }
}