using NuGet.Versioning;

namespace DotnetManager.Installation.Resolver.Exceptions;

public sealed class InstallSdkNotFoundException(NuGetVersion releaseVersion)
    : Exception($".NET release '{releaseVersion}' does not contain an SDK.")
{
    public NuGetVersion ReleaseVersion { get; } = releaseVersion;
}
