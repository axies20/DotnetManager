using NuGet.Versioning;

namespace DotnetManager.Installation.Resolver.Models.Targets;

public sealed record VersionSelector(NuGetVersion Version);
