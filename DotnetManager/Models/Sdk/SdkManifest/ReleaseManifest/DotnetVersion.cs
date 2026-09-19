using NuGet.Versioning;

namespace DotnetManager.Models.Sdk.SdkManifest.ReleaseManifest;

public sealed record DotnetVersion
{
    public required NuGetVersion Version { get; init; }
    public required IReadOnlyCollection<ReleaseFile> Artifacts { get; init; }
}