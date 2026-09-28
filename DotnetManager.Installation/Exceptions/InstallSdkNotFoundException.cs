using NuGet.Versioning;

namespace DotnetManager.Installation.Exceptions;

public sealed class InstallSdkNotFoundException(NuGetVersion releaseVersion)
    : DotnetInstallException($".NET release '{releaseVersion}' does not contain an SDK.")
{
    public NuGetVersion ReleaseVersion { get; } = releaseVersion;
}