using NuGet.Versioning;

namespace DotnetManager.Models;

public sealed record HostInstallation(
    NuGetVersion Version,
    string Path);