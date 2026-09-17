using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawIndex;

public class Signature
{
    [JsonPropertyName("expiration")]
    public DateTime expiration { get; set; }

    [JsonPropertyName("file")]
    public string file { get; set; }
}