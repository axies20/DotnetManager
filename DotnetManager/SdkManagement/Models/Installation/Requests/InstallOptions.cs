namespace DotnetManager.SdkManagement.Models.Installation.Requests;

public sealed record InstallOptions
{
    public required IReadOnlyCollection<DotnetComponent> Components { get; init; }

    public string? RuntimeIdentifier { get; init; }
}
