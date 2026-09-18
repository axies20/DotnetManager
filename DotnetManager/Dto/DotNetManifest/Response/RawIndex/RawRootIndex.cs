using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawIndex;

public class RawRootIndex
{
    [JsonPropertyName("$schema")]
    public string? Schema { get; set; }

    [JsonPropertyName("releases-index")]
    public List<RawReleasesIndex>? Releasesindex { get; set; }

    [JsonPropertyName("signature")]
    public RawIndexSignature? RawIndexSignature { get; set; }
}
