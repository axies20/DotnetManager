namespace DotnetManager.Installation.Abstractions.Installation;

internal interface IDotnetInstallationFinalizerService
{
    Task FinalizeAsync(CancellationToken cancellationToken);
}