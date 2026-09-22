using NuGet.Versioning;

namespace DotnetManager.SdkManagement.Models.Installation;

public sealed record VersionSelector(NuGetVersion Version) : InstallTarget;
