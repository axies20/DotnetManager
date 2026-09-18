using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawReleases;

public class RawReleasesAspNetCoreRuntime
{
    [JsonPropertyName("version")]
    public required string Version { get; set; }

    [JsonPropertyName("version-display")]
    public required string VersionDisplay { get; set; }

    [JsonPropertyName("version-aspnetcoremodule")]
    public required List<string> VersionAspNetCoreModule { get; set; }

    [JsonPropertyName("vs-version")]
    public required string VsVersion { get; set; }

    [JsonPropertyName("files")]
    public required List<RawReleasesFile> Files { get; set; }
}
