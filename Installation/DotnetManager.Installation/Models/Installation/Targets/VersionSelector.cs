using NuGet.Versioning;

namespace DotnetManager.Installation.Models.Installation.Targets;

public sealed record VersionSelector(NuGetVersion Version) : InstallTarget;