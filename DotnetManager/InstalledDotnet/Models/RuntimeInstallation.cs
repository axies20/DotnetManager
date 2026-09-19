using NuGet.Versioning;

namespace DotnetManager.InstalledDotnet.Models;

public sealed record RuntimeInstallation(
    string Framework,
    NuGetVersion Version,
    string Path);