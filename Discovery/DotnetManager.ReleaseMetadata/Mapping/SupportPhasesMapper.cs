using DotnetManager.ReleaseMetadata.Contracts;
using DotnetManager.ReleaseMetadata.Models;

namespace DotnetManager.ReleaseMetadata.Mapping;

public class SupportPhasesMapper
{
    public static SupportPhases Map(RawSupportPhases rawSupportPhases)
    {
        return rawSupportPhases switch
        {
            RawSupportPhases.Preview => SupportPhases.Preview,
            RawSupportPhases.GoLive => SupportPhases.GoLive,
            RawSupportPhases.Active => SupportPhases.Active,
            RawSupportPhases.Maintenance => SupportPhases.Maintenance,
            RawSupportPhases.Eol => SupportPhases.Eol,
            _ => throw new ArgumentOutOfRangeException(nameof(rawSupportPhases))
        };
    }
}