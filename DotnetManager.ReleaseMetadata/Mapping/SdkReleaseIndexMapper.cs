using DotnetManager.ReleaseMetadata.Contracts.Index;
using DotnetManager.ReleaseMetadata.Models.Index;

namespace DotnetManager.ReleaseMetadata.Mapping;

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