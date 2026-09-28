using NuGet.Versioning;

namespace DotnetManager.InstalledDotnet.Models;

public sealed record HostInstallation(NuGetVersion Version, string Path);