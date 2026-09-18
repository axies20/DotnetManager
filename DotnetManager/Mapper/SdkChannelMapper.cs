using DotnetManager.Dto.DotNetManifest.Response.RawIndex;
using DotnetManager.Models.Sdk.SdkManifest.ReleaseIndex;

namespace DotnetManager.Mapper;

public static class SdkChannelMapper
{
    public static SdkChannel Map(RawReleasesIndex releasesIndex)
    {
        return new SdkChannel
        {
            ChannelVersion = releasesIndex.ChannelVersion,
            LatestSdk = releasesIndex.LatestSdk,
            ReleasesUri = new Uri(releasesIndex.ReleasesJson),
            SupportPhase = SupportPhasesMapper.Map(releasesIndex.SupportPhase),
            ReleaseTypes = ReleaseTypeMapper.Map(releasesIndex.ReleaseType)
        };
    }
}