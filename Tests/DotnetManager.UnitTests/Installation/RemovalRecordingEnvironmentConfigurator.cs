using DotnetManager.Installation.Abstractions.UserEnvironment;

namespace DotnetManager.UnitTests.Installation;

internal sealed class RemovalRecordingEnvironmentConfigurator : IUserEnvironmentConfiguratorService
{
    public bool RemoveCalled { get; private set; }
    public string? RemovedRoot { get; private set; }

    public Task ConfigureAsync(string dotnetRoot, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task RemoveConfigurationAsync(string dotnetRoot, CancellationToken cancellationToken)
    {
        RemoveCalled = true;
        RemovedRoot = dotnetRoot;
        return Task.CompletedTask;
    }
}
