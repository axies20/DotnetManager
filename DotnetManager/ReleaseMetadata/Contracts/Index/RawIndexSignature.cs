using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts.Index;

public class RawIndexSignature
{
    [JsonPropertyName("expiration")]
    public DateTime? Expiration { get; init; }

    [JsonPropertyName("file")]
    public string? File { get; init; }
}