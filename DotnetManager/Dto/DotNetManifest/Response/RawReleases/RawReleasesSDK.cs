using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawReleases;

public class RawReleasesSDK
{
    [JsonPropertyName("version")]
    public required string Version { get; set; }

    [JsonPropertyName("version-display")]
    public required string VersionDisplay { get; set; }

    [JsonPropertyName("runtime-version")]
    public required string RuntimeVersion { get; set; }

    [JsonPropertyName("vs-version")]
    public required string VsVersion { get; set; }

    [JsonPropertyName("vs-mac-version")]
    public required string VsMacVersion { get; set; }

    [JsonPropertyName("vs-support")]
    public required string VsSupport { get; set; }

    [JsonPropertyName("vs-mac-support")]
    public required string VsMacSupport { get; set; }

    [JsonPropertyName("csharp-version")]
    public required string CsharpVersion { get; set; }

    [JsonPropertyName("fsharp-version")]
    public required string FsharpVersion { get; set; }

    [JsonPropertyName("vb-version")]
    public required string VbVersion { get; set; }

    [JsonPropertyName("files")]
    public required List<RawReleasesFile> Files { get; set; }
}