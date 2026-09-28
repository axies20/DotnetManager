using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts.Releases;

public class RawReleasesAspNetCoreRuntime
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("version-display")]
    public string? VersionDisplay { get; set; }

    [JsonPropertyName("version-aspnetcoremodule")]
    public List<string>? VersionAspNetCoreModule { get; set; }

    [JsonPropertyName("vs-version")]
    public string? VsVersion { get; set; }

    [JsonPropertyName("files")]
    public List<RawReleasesFile>? Files { get; set; }
}