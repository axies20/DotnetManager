using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts.Releases;

public class RawReleasesCveList
{
    [JsonPropertyName("cve-id")]
    public string? CveId { get; set; }

    [JsonPropertyName("cve-url")]
    public string? CveUrl { get; set; }
}
