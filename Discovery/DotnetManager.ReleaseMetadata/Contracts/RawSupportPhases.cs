using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts;

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