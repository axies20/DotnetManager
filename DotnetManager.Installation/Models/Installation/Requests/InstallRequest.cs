using DotnetManager.Installation.Models.Installation.Targets;

namespace DotnetManager.Installation.Models.Installation.Requests;

public sealed record InstallRequest
{
    public required InstallTarget Target { get; init; }

    public required InstallOptions Options { get; init; }
}