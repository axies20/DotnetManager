using System.CommandLine;

namespace DotnetManager.Cli.Commands.Remove;

internal sealed partial class RemoveCommand
{
    private readonly Option<bool> _runtime = new("--runtime", "-rt")
    {
        Description = "Remove matching .NET runtime installations."
    };

    private readonly Option<bool> _sdk = new("--sdk")
    {
        Description = "Remove matching .NET SDK installations."
    };

    private readonly Option<bool> _host = new("--host", "-ht")
    {
        Description = "Remove matching .NET host installations."
    };

    private readonly Option<bool> _asp = new("--aspnet", "-asp")
    {
        Description = "Remove matching ASP.NET Core runtime installations."
    };

    private readonly Argument<string> _version = new("version")
    {
        Description = "Major, major.minor, or exact version to remove."
    };
}