namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseIndex;

public sealed record SdkChannel
{
    public required string ChannelVersion { get; init; }
    public required string LatestSdk { get; init; }
    public required string SupportPhase { get; init; }
    public required Uri ReleasesUri { get; init; }
}
