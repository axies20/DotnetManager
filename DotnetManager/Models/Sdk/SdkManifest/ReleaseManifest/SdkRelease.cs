namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;

public sealed record SdkRelease
{
    public required string ReleaseVersion { get; init; }
    public required DateOnly ReleaseDate { get; init; }
    public required bool Security { get; init; }
    public required SdkVersion Sdk { get; init; }
}
