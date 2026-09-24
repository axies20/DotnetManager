namespace DotnetManager.SdkManagement.Models.Installation.Requests;

public sealed record InstallOptions
{
    public required IReadOnlyCollection<InstallComponent> Components { get; init; }

    public bool Security { get; init; } = true;

    public string? RuntimeIdentifier { get; init; }
}
