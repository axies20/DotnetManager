using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts.Releases;

public class RawReleasesSignature
{
    [JsonPropertyName("expiration")]
    public DateTime? Expiration { get; set; }

    [JsonPropertyName("file")]
    public string? File { get; set; }
}
