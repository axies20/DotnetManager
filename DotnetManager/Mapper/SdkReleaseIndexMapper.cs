using DotnetManager.Dto.DotNetManifest.Response.RawIndex;
using DotnetManager.Helper;
using DotnetManager.Models.Sdk.SdkManifest.ReleaseIndex;

namespace DotnetManager.Mapper;

public static class SdkReleaseIndexMapper
{
    public static SdkReleaseIndex Map(RawRootIndex index)
    {
        ArgumentNullException.ThrowIfNull(index);
        var channels = MappingGuard.Required(index.Releasesindex)
            .Select(SdkChannelMapper.Map).ToList();

        return new SdkReleaseIndex(channels);
    }
}