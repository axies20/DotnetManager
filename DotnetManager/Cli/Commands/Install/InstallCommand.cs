using System.CommandLine;
using DotnetManager.Cli.Abstractions;
using DotnetManager.ReleaseMetadata.Models;
using NuGet.Versioning;

namespace DotnetManager.Cli.Commands.Install;

public class InstallCommand : ICommand
{

    private readonly Command _latest = new("latest",
        "Install the latest version of the specified release type and support phase");

    private readonly Option<ReleaseTypes> _releaseType = new("--release-type",
        "Install the latest version of the specified release type");

    private readonly Option<SupportPhases> _supportPhase = new("--support-phase",
        "Install the latest version of the specified support phase");

    private readonly Option<bool> _runtime = new("--runtime",
        "Install the .NET runtime instead of the SDK");

    private readonly Option<bool> _aspnet = new("--aspnet",
        "Install the ASP.NET Core runtime instead of the SDK");

    private readonly Argument<string> _version = new("version")
    {
        Description = "SDK channel or exact version to install"
    };


    public Command Initialize()
    {
        var command = new Command("install",
            "Download and install a .NET SDK selected by channel, support policy, or exact version into the configured installation directory.");
        command.Arguments.Add(_version);

        command.Subcommands.Add(_latest);

        command.Options.Add(_releaseType);
        command.Options.Add(_supportPhase);

        command.SetAction(ExecuteInstall);
        _latest.SetAction(ExecuteLatest);
        return command;
    }

    private Task ExecuteLatest(ParseResult result)
    {
        var runtime = result.GetValue(_runtime);
        var aspnet = result.GetValue(_aspnet);
    }

    private Task ExecuteInstall(ParseResult result)
    {
        var value = result.GetValue(_version);

        if (!NuGetVersion.TryParse(value, out var version))
        {
            Console.Error.WriteLine($"Invalid version: {value}");
            return Task.CompletedTask;
        }

        var runtime = result.GetValue(_runtime);
        var aspnet = result.GetValue(_aspnet);
    }
}