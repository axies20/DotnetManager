namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;

public sealed record SdkVersion
{
    public required string Version { get; init; }
    public required IReadOnlyCollection<SdkReleaseArtifact> Artifacts { get; init; }
}