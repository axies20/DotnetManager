using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawReleases;

public class RawReleasesCveList
{
    [JsonPropertyName("cve-id")]
    public required string CveId { get; set; }

    [JsonPropertyName("cve-url")]
    public required string CveUrl { get; set; }
}