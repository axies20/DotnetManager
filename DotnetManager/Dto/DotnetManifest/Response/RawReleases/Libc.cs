using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawReleases;

public class Libc
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("architectures")]
    public List<string> Architectures { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }
}