using DotnetManager.SdkManagement.Models.Installation.Targets;

namespace DotnetManager.SdkManagement.Models.Installation.Requests;

public sealed record InstallRequest
{
    public required InstallTarget Target { get; init; }

    public required InstallOptions Options { get; init; }
}
