using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawIndex;

public class RawReleasesIndex
{
    [JsonPropertyName("channel-version")]
    public required string ChannelVersion { get; set; }

    [JsonPropertyName("latest-release")]
    public required string LatestRelease { get; set; }

    [JsonPropertyName("latest-release-date")]
    public required string LatestReleaseDate { get; set; }

    [JsonPropertyName("security")]
    public bool Security { get; set; }

    [JsonPropertyName("latest-runtime")]
    public required string LatestRuntime { get; set; }

    [JsonPropertyName("latest-sdk")]
    public required string LatestSdk { get; set; }

    [JsonPropertyName("product")]
    public required string Product { get; set; }

    [JsonPropertyName("support-phase")]
    [JsonConverter(typeof(JsonStringEnumConverter<RawSupportPhases>))]
    public required RawSupportPhases SupportPhase { get; set; }

    [JsonPropertyName("release-type")]
    [JsonConverter(typeof(JsonStringEnumConverter<RawReleaseTypes>))]
    public required RawReleaseTypes ReleaseType { get; set; }

    [JsonPropertyName("releases.json")]
    public required string ReleasesJson { get; set; }

    [JsonPropertyName("supported-os.json")]
    public required string SupportedOsJson { get; set; }

    [JsonPropertyName("eol-date")]
    public required string EolDate { get; set; }
}