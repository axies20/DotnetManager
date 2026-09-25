using DotnetManager.Installation.Abstractions.InstallPaths;

namespace DotnetManager.UnitTests.Installation;

internal sealed class RemovalPathProvider(string installRoot) : IDotnetInstallPathProviderService
{
    public string GetInstallDirectory() => installRoot;

    public string? GetExecutableLinkPath() => null;
}
