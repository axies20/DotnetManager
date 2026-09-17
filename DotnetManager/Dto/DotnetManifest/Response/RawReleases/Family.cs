using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawReleases;

public class Family
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("distributions")]
    public List<Distribution> Distributions { get; set; }
}