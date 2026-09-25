namespace DotnetManager.SdkManagement.Abstractions.Installation;

public interface IDotnetInstallationFinalizerService
{
    Task FinalizeAsync(CancellationToken cancellationToken);
}
