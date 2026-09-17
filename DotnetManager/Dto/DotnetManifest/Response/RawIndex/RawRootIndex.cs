using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawIndex;

public class RawRootIndex
{
    [JsonPropertyName("$schema")] 
    public string schema { get; set; }

    [JsonPropertyName("releases-index")]
    public List<ReleasesIndex> releasesindex { get; set; }

    [JsonPropertyName("signature")]
    public Signature signature { get; set; }
}