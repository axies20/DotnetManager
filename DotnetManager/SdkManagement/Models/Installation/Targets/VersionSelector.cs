using NuGet.Versioning;

namespace DotnetManager.SdkManagement.Models.Installation.Targets;

public sealed record VersionSelector(NuGetVersion Version) : InstallTarget;
