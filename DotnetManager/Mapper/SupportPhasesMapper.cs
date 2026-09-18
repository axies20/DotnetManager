using DotnetManager.Dto.DotNetManifest.Response;
using DotnetManager.Models;

namespace DotnetManager.Mapper;

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