namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseIndex;

public sealed record SdkChannel
{
    public required string ChannelVersion { get; init; }
    public required string LatestSdk { get; init; }
    public required SupportPhases SupportPhase { get; init; }
    public required ReleaseTypes ReleaseTypes { get; set; }
    public required Uri ReleasesUri { get; init; }
}