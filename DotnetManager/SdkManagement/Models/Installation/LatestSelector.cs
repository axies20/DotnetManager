using DotnetManager.ReleaseMetadata.Models;

namespace DotnetManager.SdkManagement.Models.Installation;

public sealed record LatestSelector(ReleaseTypes? ReleaseType, SupportPhases? SupportPhase);