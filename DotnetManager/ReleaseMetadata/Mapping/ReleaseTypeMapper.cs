using DotnetManager.ReleaseMetadata.Contracts;
using DotnetManager.ReleaseMetadata.Models;

namespace DotnetManager.ReleaseMetadata.Mapping;

public static class ReleaseTypeMapper
{
    public static ReleaseTypes Map(RawReleaseTypes rawReleaseTypes)
    {
        return rawReleaseTypes switch
        {
            RawReleaseTypes.Sts => ReleaseTypes.Sts,
            RawReleaseTypes.Lts => ReleaseTypes.Lts,
            _ => throw new ArgumentOutOfRangeException(nameof(rawReleaseTypes))
        };
    }
}
