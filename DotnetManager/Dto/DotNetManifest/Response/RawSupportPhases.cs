using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response;

public enum RawSupportPhases
{
    [JsonStringEnumMemberName("preview")]
    Preview,

    [JsonStringEnumMemberName("go-live")]
    GoLive,

    [JsonStringEnumMemberName("active")]
    Active,

    [JsonStringEnumMemberName("maintenance")]
    Maintenance,

    [JsonStringEnumMemberName("eol")]
    Eol
}