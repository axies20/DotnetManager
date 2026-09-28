using NuGet.Versioning;

namespace DotnetManager.InstalledDotnet.Models;

public sealed record SdkInstallation(NuGetVersion Version, string Path);