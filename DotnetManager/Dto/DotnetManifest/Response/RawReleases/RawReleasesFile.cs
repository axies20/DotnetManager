using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawReleases;

public class RawReleasesFile
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("rid")]
    public required string Rid { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("hash")]
    public required string Hash { get; set; }

    [JsonPropertyName("akams")]
    public required string Akams { get; set; }
}
