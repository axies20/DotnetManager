namespace DotnetManager.Installation.Abstractions.InstallPaths;

internal interface IDotnetInstallPathProviderService
{
    string GetInstallDirectory();
}
