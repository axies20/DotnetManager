using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawReleases;

public class Distribution
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyName("lifecycle")]
    public string Lifecycle { get; set; }

    [JsonPropertyName("architectures")]
    public List<string> Architectures { get; set; }

    [JsonPropertyName("supported-versions")]
    public List<string> SupportedVersions { get; set; }

    [JsonPropertyName("notes")]
    public List<string> Notes { get; set; }

    [JsonPropertyName("unsupported-versions")]
    public List<string> UnsupportedVersions { get; set; }
}