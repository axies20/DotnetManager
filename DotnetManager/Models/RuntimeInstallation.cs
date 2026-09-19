using NuGet.Versioning;

namespace DotnetManager.Models;

public sealed record RuntimeInstallation(
    string Framework,
    NuGetVersion Version,
    string Path);