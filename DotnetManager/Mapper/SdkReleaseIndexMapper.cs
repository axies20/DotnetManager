using DotnetManager.Dto.DotNetManifest.Response.RawIndex;
using DotnetManager.Models.Sdk.SdkManifest.ReleaseIndex;

namespace DotnetManager.Mapper;

public static class SdkReleaseIndexMapper
{
    public static SdkReleaseIndex Map(RawRootIndex index)
    {
        var channels = index.Releasesindex.Select(SdkChannelMapper.Map).ToList();

        return new SdkReleaseIndex(channels);
    }
}