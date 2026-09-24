namespace DotnetManager.SdkManagement.Models.Installation.Requests;

public sealed record InstallOptions
{
    public required IReadOnlyCollection<InstallComponent> Components { get; init; }

    public string? RuntimeIdentifier { get; init; }
}