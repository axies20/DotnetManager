using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawReleases;

public class RawReleasesRuntime
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("version-display")]
    public string? VersionDisplay { get; set; }

    [JsonPropertyName("vs-version")]
    public string? VsVersion { get; set; }

    [JsonPropertyName("vs-mac-version")]
    public string? VsMacVersion { get; set; }

    [JsonPropertyName("files")]
    public List<RawReleasesFile>? Files { get; set; }
}
