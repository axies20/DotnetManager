using DotnetManager.ReleaseMetadata.Models;

namespace DotnetManager.SdkManagement.Models.Installation.Targets;

public sealed record LatestSelector(
    ReleaseTypes? ReleaseType,
    SupportPhases? SupportPhase,
    bool SecurityOnly) : InstallTarget;