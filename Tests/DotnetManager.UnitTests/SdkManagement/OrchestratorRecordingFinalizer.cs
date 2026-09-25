using DotnetManager.Installation.Abstractions.Installation;

namespace DotnetManager.UnitTests.SdkManagement;

internal sealed class OrchestratorRecordingFinalizer : IDotnetInstallationFinalizerService
{
    public bool Called { get; private set; }

    public Task FinalizeAsync(CancellationToken cancellationToken)
    {
        Called = true;
        return Task.CompletedTask;
    }
}
