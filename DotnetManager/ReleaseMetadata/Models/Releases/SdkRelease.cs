using NuGet.Versioning;

namespace DotnetManager.ReleaseMetadata.Models.Releases;

public sealed record SdkRelease
{
    public required NuGetVersion ReleaseVersion { get; init; }

    public required DateOnly ReleaseDate { get; init; }

    public required bool Security { get; init; }

    public required DotnetVersion Runtime { get; init; }

    public required DotnetVersion AspNetCoreRuntime { get; init; }

    public required IReadOnlyList<DotnetVersion> Sdks { get; init; }
}