using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotnetManifest.Response.RawReleases;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

public class RawRelease
{
    [JsonPropertyName("channel-version")]
    public string ChannelVersion { get; set; }

    [JsonPropertyName("last-updated")]
    public string LastUpdated { get; set; }

    [JsonPropertyName("families")]
    public List<Family> Families { get; set; }

    [JsonPropertyName("libc")]
    public List<Libc> Libc { get; set; }

    [JsonPropertyName("notes")]
    public List<string> Notes { get; set; }
}

