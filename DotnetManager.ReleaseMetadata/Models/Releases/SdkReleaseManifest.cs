using NuGet.Versioning;

namespace DotnetManager.ReleaseMetadata.Models.Releases;

public sealed record SdkReleaseManifest
{
    public required NuGetVersion ChannelVersion { get; init; }

    public required NuGetVersion LatestSdk { get; init; }

    public required NuGetVersion LatestRelease { get; init; }

    public required NuGetVersion LatestRuntime { get; init; }

    public required SupportPhases SupportPhase { get; init; }

    public required ReleaseTypes ReleaseType { get; init; }

    public required IReadOnlyCollection<SdkRelease> Releases { get; init; }
}