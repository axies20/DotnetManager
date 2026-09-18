using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawReleases;

public class RawReleasesWindowsDesktop
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("version-display")]
    public string? VersionDisplay { get; set; }

    [JsonPropertyName("files")]
    public List<RawReleasesFile>? Files { get; set; }
}
