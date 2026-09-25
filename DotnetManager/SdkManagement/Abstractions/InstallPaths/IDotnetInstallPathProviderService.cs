namespace DotnetManager.SdkManagement.Abstractions.InstallPaths;

public interface IDotnetInstallPathProviderService
{
    string GetInstallDirectory();
    string? GetExecutableLinkPath();
}
