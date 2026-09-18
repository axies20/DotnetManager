using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawIndex;

public class RawIndexSignature
{
    [JsonPropertyName("expiration")]
    public DateTime Expiration { get; init; }

    [JsonPropertyName("file")]
    public required string File { get; init; }
}
