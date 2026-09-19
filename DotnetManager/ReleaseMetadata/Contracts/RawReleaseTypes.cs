using System.Text.Json.Serialization;

namespace DotnetManager.ReleaseMetadata.Contracts;

public enum RawReleaseTypes
{
    [JsonStringEnumMemberName("sts")]
    Sts,

    [JsonStringEnumMemberName("lts")]
    Lts
}