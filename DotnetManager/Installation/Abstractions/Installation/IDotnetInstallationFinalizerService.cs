namespace DotnetManager.Installation.Abstractions.Installation;

public interface IDotnetInstallationFinalizerService
{
    Task FinalizeAsync(CancellationToken cancellationToken);
}
