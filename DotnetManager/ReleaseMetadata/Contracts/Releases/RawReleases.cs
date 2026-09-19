using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts.Releases;

public class RawReleases
{
    [JsonPropertyName("release-date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("release-version")]
    public string? ReleaseVersion { get; set; }

    [JsonPropertyName("security")]
    public bool? Security { get; set; }

    [JsonPropertyName("cve-list")]
    public List<RawReleasesCveList>? CveList { get; set; }

    [JsonPropertyName("release-notes")]
    public string? ReleaseNotes { get; set; }

    [JsonPropertyName("runtime")]
    public RawReleasesRuntime? Runtime { get; set; }

    [JsonPropertyName("sdk")]
    public RawReleasesSDK? Sdk { get; set; }

    [JsonPropertyName("sdks")]
    public List<RawReleasesSDK>? Sdks { get; set; }

    [JsonPropertyName("aspnetcore-runtime")]
    public RawReleasesAspNetCoreRuntime? RawAspNetCoreRuntime { get; set; }

    [JsonPropertyName("windowsdesktop")]
    public RawReleasesWindowsDesktop? Windowsdesktop { get; set; }
}
