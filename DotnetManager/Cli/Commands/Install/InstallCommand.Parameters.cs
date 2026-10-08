using System.CommandLine;
using DotnetManager.ReleaseMetadata.Models;

namespace DotnetManager.Cli.Commands.Install;

internal sealed partial class InstallCommand
{
    private readonly Command _latest = new("latest",
        "Install the latest version of the specified release type and support phase");

    private readonly Option<ReleaseTypes?> _releaseType = new("--release-type", "-t")
    {
        Description = "Install the latest version of the specified release type"
    };

    private readonly Option<SupportPhases?> _supportPhase = new("--support-phase", "-sp")
    {
        Description = "Install the latest version of the specified support phase"
    };

    private readonly Option<bool> _runtime = new("--runtime", "-r")
    {
        Description = "Install the .NET runtime instead of the SDK",
        Recursive = true
    };

    private readonly Option<bool> _aspnet = new("--aspnet", "-asp")
    {
        Description = "Install the ASP.NET Core runtime instead of the SDK",
        Recursive = true
    };

    private readonly Option<string?> _rid = new("--rid")
    {
        Description = "Install the .NET runtime with the specified runtime identifier",
        Recursive = true
    };

    private readonly Argument<string> _version = new("version")
    {
        Description = "Exact .NET release version to install"
    };
}