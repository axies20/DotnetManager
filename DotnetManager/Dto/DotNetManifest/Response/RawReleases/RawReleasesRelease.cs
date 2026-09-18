using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response.RawReleases;

public class RawReleasesRelease
{
    [JsonPropertyName("release-date")]
    public required string ReleaseDate { get; set; }

    [JsonPropertyName("release-version")]
    public required string ReleaseVersion { get; set; }

    [JsonPropertyName("security")]
    public bool Security { get; set; }

    [JsonPropertyName("cve-list")]
    public required List<RawReleasesCveList> CveList { get; set; }

    [JsonPropertyName("release-notes")]
    public required string ReleaseNotes { get; set; }

    [JsonPropertyName("runtime")]
    public required RawReleasesRuntime Runtime { get; set; }

    [JsonPropertyName("sdk")]
    public required RawReleasesSDK Sdk { get; set; }

    [JsonPropertyName("sdks")]
    public required List<RawReleasesSDK> Sdks { get; set; }

    [JsonPropertyName("aspnetcore-runtime")]
    public required RawReleasesAspNetCoreRuntime RawAspNetCoreRuntime { get; set; }

    [JsonPropertyName("windowsdesktop")]
    public required RawReleasesWindowsDesktop Windowsdesktop { get; set; }
}
