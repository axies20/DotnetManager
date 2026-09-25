using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.Installation.Abstractions.Removal;
using DotnetManager.Installation.Models;

namespace DotnetManager.Cli.Commands;

public class RemoveCommand : ICommand
{
    private readonly Option<bool> _runtime = new("--runtime", "-rt")
    {
        Description = "Remove the runtime installation along  with the SDK."
    };

    private readonly Option<bool> _sdk = new("--sdk")
    {
        Description = "Remove the SDK installation along with the runtime when it is no longer required."
    };

    private readonly Option<bool> _host = new("--host", "-ht")
    {
        Description = "Remove the host installation along with the SDK when it is no longer required."
    };

    private readonly Option<bool> _asp = new("--aspnet", "-asp")
    {
        Description = "Remove the ASP.NET Core installation along with the SDK when it is no longer required."
    };

    private readonly Argument<string> _version = new("version")
    {
        Description = "The version to remove."
    };

    private readonly IDotnetRemovalService _removalService;

    public RemoveCommand(IDotnetRemovalService removalService)
    {
        _removalService = removalService;
    }

    public Command Initialize()
    {
        var command = new Command("remove",
            "Stop tracking a .NET SDK channel or exact pinned version and remove its managed installation when it is no longer required.");
        command.Options.Add(_runtime);
        command.Options.Add(_sdk);
        command.Options.Add(_host);
        command.Options.Add(_asp);
        command.Arguments.Add(_version);
        command.SetAction(Execute);
        return command;
    }

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