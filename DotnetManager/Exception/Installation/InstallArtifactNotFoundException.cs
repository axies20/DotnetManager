using NuGet.Versioning;

namespace DotnetManager.Exception.Installation;

public sealed class InstallArtifactNotFoundException(
    string componentName,
    NuGetVersion version,
    string runtimeIdentifier)
    : DotnetInstallException(
        $"{componentName} {version} is not available for RID '{runtimeIdentifier}'.")
{
    public string ComponentName { get; } = componentName;

    public NuGetVersion Version { get; } = version;

    public string RuntimeIdentifier { get; } = runtimeIdentifier;
}