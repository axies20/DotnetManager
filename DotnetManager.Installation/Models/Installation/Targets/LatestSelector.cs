using DotnetManager.ReleaseMetadata.Models;

namespace DotnetManager.Installation.Models.Installation.Targets;

public sealed record LatestSelector(
    ReleaseTypes? ReleaseType,
    SupportPhases? SupportPhase,
    bool SecurityOnly) : InstallTarget;