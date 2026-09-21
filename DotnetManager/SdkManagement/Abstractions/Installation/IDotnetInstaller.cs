namespace DotnetManager.SdkManagement.Abstractions.Installation;

public interface IDotnetInstaller
{
    Task InstallAsync(string sourcePath, CancellationToken cancellationToken);
}