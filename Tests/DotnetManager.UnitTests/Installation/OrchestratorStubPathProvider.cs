using DotnetManager.Installation.Abstractions.InstallPaths;

namespace DotnetManager.UnitTests.Installation;

internal sealed class OrchestratorStubPathProvider(string path) : IDotnetInstallPathProviderService
{
    public string GetInstallDirectory()
    {
        return path;
    }
}