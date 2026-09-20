using DotnetManager.ReleaseMetadata.Contracts.Index;
using DotnetManager.ReleaseMetadata.Models.Index;
using NuGet.Versioning;

namespace DotnetManager.ReleaseMetadata.Mapping;

public static class SdkChannelMapper
{
    public static SdkChannel Map(RawReleasesIndex releasesIndex)
    {
        ArgumentNullException.ThrowIfNull(releasesIndex);
        return new SdkChannel
        {
            ChannelVersion = NuGetVersion.Parse(MappingGuard.Required(releasesIndex.ChannelVersion)),
            Security = MappingGuard.Required(releasesIndex.Security),
            LatestSdk = MappingGuard.Required(releasesIndex.LatestSdk),
            ReleasesUri = new Uri(MappingGuard.Required(releasesIndex.ReleasesJson)),
            SupportPhase = SupportPhasesMapper.Map(MappingGuard.Required(releasesIndex.SupportPhase)),
            ReleaseTypes = ReleaseTypeMapper.Map(MappingGuard.Required(releasesIndex.ReleaseType))
        };
    }
}