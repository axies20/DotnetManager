using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawIndex;

public class RawRootIndex
{
    [JsonPropertyName("$schema")]
    public required string Schema { get; set; }

    [JsonPropertyName("releases-index")]
    public required List<RawReleasesIndex> Releasesindex { get; set; }

    [JsonPropertyName("signature")]
    public required RawIndexSignature RawIndexSignature { get; set; }
}