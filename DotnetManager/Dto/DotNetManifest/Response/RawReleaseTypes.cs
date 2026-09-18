using System.Text.Json.Serialization;

namespace DotnetManager.Dto.DotNetManifest.Response;

public enum RawReleaseTypes
{
    [JsonStringEnumMemberName("sts")]
    Sts,

    [JsonStringEnumMemberName("lts")]
    Lts
}