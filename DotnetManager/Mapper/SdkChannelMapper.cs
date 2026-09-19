using DotnetManager.Dto.DotNetManifest.Response.RawIndex;
using DotnetManager.Helper;
using DotnetManager.Models.Sdk.SdkManifest.ReleaseIndex;
using NuGet.Versioning;

namespace DotnetManager.Mapper;

public static class SdkChannelMapper
{
    public static SdkChannel Map(RawReleasesIndex releasesIndex)
    {
        ArgumentNullException.ThrowIfNull(releasesIndex);
        return new SdkChannel
        {
            ChannelVersion = NuGetVersion.Parse(MappingGuard.Required(releasesIndex.ChannelVersion)),
            LatestSdk = MappingGuard.Required(releasesIndex.LatestSdk),
            ReleasesUri = new Uri(MappingGuard.Required(releasesIndex.ReleasesJson)),
            SupportPhase = SupportPhasesMapper.Map(MappingGuard.Required(releasesIndex.SupportPhase)),
            ReleaseTypes = ReleaseTypeMapper.Map(MappingGuard.Required(releasesIndex.ReleaseType))
        };
    }
}