using NuGet.Versioning;

namespace DotnetManager.ReleaseMetadata.Models.Index;

public sealed record SdkChannel
{
    public required NuGetVersion ChannelVersion { get; init; }
    public required bool Security { get; init; }
    public required string LatestSdk { get; init; }
    public required SupportPhases SupportPhase { get; init; }
    public required ReleaseTypes ReleaseTypes { get; set; }
    public required Uri ReleasesUri { get; init; }
}