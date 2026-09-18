using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawReleases;

public class RawReleasesRuntime
{
    [JsonPropertyName("version")]
    public required string Version { get; set; }

    [JsonPropertyName("version-display")]
    public required string VersionDisplay { get; set; }

    [JsonPropertyName("vs-version")]
    public required string VsVersion { get; set; }

    [JsonPropertyName("vs-mac-version")]
    public required string VsMacVersion { get; set; }

    [JsonPropertyName("files")]
    public required List<RawReleasesFile> Files { get; set; }
}
