namespace DotnetManager.SdkManagement.Models.Installation;

public sealed record InstallRequest
{
    public required InstallTarget Target { get; init; }

    public string? RuntimeIdentifier { get; init; }

    public bool Security { get; init; } = true;

    public required IReadOnlyCollection<InstallComponent> Components { get; init; }
}
