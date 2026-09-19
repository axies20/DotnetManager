using NuGet.Versioning;

namespace DotnetManager.Models;

public sealed record SdkInstallation(NuGetVersion Version, string Path);