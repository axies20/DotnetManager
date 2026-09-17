using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawIndex;

public class ReleasesIndex
{
    [JsonPropertyName("channel-version")]
    public string ChannelVersion { get; set; }

    [JsonPropertyName("latest-release")]
    public string LatestRelease { get; set; }

    [JsonPropertyName("latest-release-date")]
    public string LatestReleaseDate { get; set; }

    [JsonPropertyName("security")]
    public bool Security { get; set; }

    [JsonPropertyName("latest-runtime")]
    public string LatestRuntime { get; set; }

    [JsonPropertyName("latest-sdk")]
    public string LatestSdk { get; set; }

    [JsonPropertyName("product")]
    public string Product { get; set; }

    [JsonPropertyName("support-phase")]
    public string SupportPhase { get; set; }

    [JsonPropertyName("release-type")]
    public string ReleaseType { get; set; }

    [JsonPropertyName("releases.json")]
    public string ReleasesJson { get; set; }

    [JsonPropertyName("supported-os.json")]
    public string SupportedOsJson { get; set; }

    [JsonPropertyName("eol-date")]
    public string EolDate { get; set; }
}