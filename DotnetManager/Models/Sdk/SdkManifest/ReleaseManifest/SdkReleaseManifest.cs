namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;

public sealed record SdkReleaseManifest
{
    public required string ChannelVersion { get; init; }
    public required string LatestSdk { get; init; }
    public required string SupportPhase { get; init; }
    public required string ReleaseType { get; init; }
    public required IReadOnlyCollection<SdkRelease> Releases { get; init; }
}
