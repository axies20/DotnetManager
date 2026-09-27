using DotnetManager.Installation.Abstractions.InstallPaths;

namespace DotnetManager.UnitTests.SdkManagement;

internal sealed class OrchestratorStubPathProvider(string path) : IDotnetInstallPathProviderService
{
    public string GetInstallDirectory()
    {
        return path;
    }

    public string? GetExecutableLinkPath()
    {
        return null;
    }
}