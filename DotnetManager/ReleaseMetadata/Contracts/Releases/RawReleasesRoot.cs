using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts.Releases;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

public class RawReleasesRoot
{
    [JsonPropertyName("channel-version")]
    public string? ChannelVersion { get; set; }

    [JsonPropertyName("latest-release")]
    public string? LatestRelease { get; set; }

    [JsonPropertyName("latest-release-date")]
    public string? LatestReleaseDate { get; set; }

    [JsonPropertyName("latest-runtime")]
    public string? LatestRuntime { get; set; }

    [JsonPropertyName("latest-sdk")]
    public string? LatestSdk { get; set; }

    [JsonPropertyName("support-phase")]
    [JsonConverter(typeof(JsonStringEnumConverter<RawSupportPhases>))]
    public RawSupportPhases? SupportPhase { get; set; }

    [JsonPropertyName("release-type")]
    [JsonConverter(typeof(JsonStringEnumConverter<RawReleaseTypes>))]
    public RawReleaseTypes? ReleaseType { get; set; }

    [JsonPropertyName("lifecycle-policy")]
    public string? LifecyclePolicy { get; set; }

    [JsonPropertyName("releases")]
    public List<RawReleases>? Releases { get; set; }

    [JsonPropertyName("signature")]
    public RawReleasesSignature? Signature { get; set; }
}