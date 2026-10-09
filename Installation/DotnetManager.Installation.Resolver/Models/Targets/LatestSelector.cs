using DotnetManager.ReleaseMetadata.Models;

namespace DotnetManager.Installation.Resolver.Models.Targets;

public sealed record LatestSelector(ReleaseTypes? ReleaseType, SupportPhases? SupportPhase);