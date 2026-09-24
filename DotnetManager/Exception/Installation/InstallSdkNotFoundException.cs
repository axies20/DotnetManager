using NuGet.Versioning;

namespace DotnetManager.Exception.Installation;

public sealed class InstallSdkNotFoundException(NuGetVersion releaseVersion)
    : DotnetInstallException($".NET release '{releaseVersion}' does not contain an SDK.")
{
    public NuGetVersion ReleaseVersion { get; } = releaseVersion;
}