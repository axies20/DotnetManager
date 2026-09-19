using NuGet.Versioning;

namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;

public sealed record SdkReleaseManifest
{
    public required NuGetVersion ChannelVersion { get; init; }
    public required NuGetVersion LatestSdk { get; init; }
    public required SupportPhases SupportPhase { get; init; }
    public required ReleaseTypes ReleaseType { get; init; }
    public required IReadOnlyCollection<SdkRelease> Releases { get; init; }
}