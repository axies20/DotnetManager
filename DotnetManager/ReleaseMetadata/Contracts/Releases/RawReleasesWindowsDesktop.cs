using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts.Releases;

public class RawReleasesWindowsDesktop
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("version-display")]
    public string? VersionDisplay { get; set; }

    [JsonPropertyName("files")]
    public List<RawReleasesFile>? Files { get; set; }
}
