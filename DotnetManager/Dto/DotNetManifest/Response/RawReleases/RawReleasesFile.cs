using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawReleases;

public class RawReleasesFile
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("rid")]
    public string? Rid { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("hash")]
    public string? Hash { get; set; }

    [JsonPropertyName("akams")]
    public string? Akams { get; set; }
}
