using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts.Index;

public class RawRootIndex
{
    [JsonPropertyName("$schema")]
    public string? Schema { get; set; }

    [JsonPropertyName("releases-index")]
    public List<RawReleasesIndex>? Releasesindex { get; set; }

    [JsonPropertyName("signature")]
    public RawIndexSignature? RawIndexSignature { get; set; }
}