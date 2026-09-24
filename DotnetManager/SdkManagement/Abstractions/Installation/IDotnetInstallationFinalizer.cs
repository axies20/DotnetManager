namespace DotnetManager.SdkManagement.Abstractions.Installation;

public interface IDotnetInstallationFinalizer
{
    Task FinalizeAsync(CancellationToken cancellationToken);
}