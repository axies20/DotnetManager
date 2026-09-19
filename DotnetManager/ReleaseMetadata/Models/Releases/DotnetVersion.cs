using NuGet.Versioning;

namespace DotnetManager.ReleaseMetadata.Models.Releases;

public sealed record DotnetVersion
{
    public required NuGetVersion Version { get; init; }
    public required IReadOnlyCollection<ReleaseFile> Artifacts { get; init; }
}