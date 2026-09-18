using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawReleases;

public class RawReleasesWindowsDesktop
{
    [JsonPropertyName("version")]
    public required string Version { get; set; }

    [JsonPropertyName("version-display")]
    public required string VersionDisplay { get; set; }

    [JsonPropertyName("files")]
    public required List<RawReleasesFile> Files { get; set; }
}
