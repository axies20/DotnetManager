using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawIndex;

public class RawReleasesIndex
{
    [JsonPropertyName("channel-version")]
    public string? ChannelVersion { get; set; }

    [JsonPropertyName("latest-release")]
    public string? LatestRelease { get; set; }

    [JsonPropertyName("latest-release-date")]
    public string? LatestReleaseDate { get; set; }

    [JsonPropertyName("security")]
    public bool? Security { get; set; }

    [JsonPropertyName("latest-runtime")]
    public string? LatestRuntime { get; set; }

    [JsonPropertyName("latest-sdk")]
    public string? LatestSdk { get; set; }

    [JsonPropertyName("product")]
    public string? Product { get; set; }

    [JsonPropertyName("support-phase")]
    [JsonConverter(typeof(JsonStringEnumConverter<RawSupportPhases>))]
    public RawSupportPhases? SupportPhase { get; set; }

    [JsonPropertyName("release-type")]
    [JsonConverter(typeof(JsonStringEnumConverter<RawReleaseTypes>))]
    public RawReleaseTypes? ReleaseType { get; set; }

    [JsonPropertyName("releases.json")]
    public string? ReleasesJson { get; set; }

    [JsonPropertyName("supported-os.json")]
    public string? SupportedOsJson { get; set; }

    [JsonPropertyName("eol-date")]
    public string? EolDate { get; set; }
}
