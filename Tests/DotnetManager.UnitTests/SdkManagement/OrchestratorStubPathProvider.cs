using DotnetManager.Installation.Abstractions.InstallPaths;

namespace DotnetManager.UnitTests.SdkManagement;

internal sealed class OrchestratorStubPathProvider(string path) : IDotnetInstallPathProviderService
{
    public string GetInstallDirectory() => path;

    public string? GetExecutableLinkPath() => null;
}
