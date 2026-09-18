namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;

public sealed record SdkRelease
{
    public required string ReleaseVersion { get; init; }
    public required DateOnly ReleaseDate { get; init; }
    public required bool Security { get; init; }
    public required DotnetVersion Runtime { get; init; }
    public required IReadOnlyList<DotnetVersion> Sdks { get; init; }
    public required DotnetVersion AspNetCoreRuntime { get; init; }
}