using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts.Releases;

public class RawReleasesSDK
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("version-display")]
    public string? VersionDisplay { get; set; }

    [JsonPropertyName("runtime-version")]
    public string? RuntimeVersion { get; set; }

    [JsonPropertyName("vs-version")]
    public string? VsVersion { get; set; }

    [JsonPropertyName("vs-mac-version")]
    public string? VsMacVersion { get; set; }

    [JsonPropertyName("vs-support")]
    public string? VsSupport { get; set; }

    [JsonPropertyName("vs-mac-support")]
    public string? VsMacSupport { get; set; }

    [JsonPropertyName("csharp-version")]
    public string? CsharpVersion { get; set; }

    [JsonPropertyName("fsharp-version")]
    public string? FsharpVersion { get; set; }

    [JsonPropertyName("vb-version")]
    public string? VbVersion { get; set; }

    [JsonPropertyName("files")]
    public List<RawReleasesFile>? Files { get; set; }
}