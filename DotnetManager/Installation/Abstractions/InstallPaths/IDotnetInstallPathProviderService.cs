namespace DotnetManager.Installation.Abstractions.InstallPaths;

public interface IDotnetInstallPathProviderService
{
    string GetInstallDirectory();
    string? GetExecutableLinkPath();
}
