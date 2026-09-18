using DotnetManager.Dto.DotNetManifest.Response;
using DotnetManager.Models;

namespace DotnetManager.Mapper;

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