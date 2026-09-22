namespace DotnetManager.SdkManagement.Abstractions.Installation;

public interface IDotnetInstaller
{
    Task InstallAsync(CancellationToken cancellationToken = default);
}
