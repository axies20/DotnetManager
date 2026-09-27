using System.CommandLine;

namespace DotnetManager.Cli.Commands.Remove;

public partial class RemoveCommand
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
}
