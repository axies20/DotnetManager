namespace DotnetManager.SdkManagement.Abstractions.InstallPaths;

public interface IDotnetInstallPathProvider
{
    string GetInstallDirectory();
    string? GetExecutableLinkPath();
}