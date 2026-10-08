using DotnetManager.Installation.Exceptions;
using DotnetManager.Installation.Services.Installation;
using DotnetManager.UserEnvironment.Abstraction;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotnetManager.UnitTests.Installation;

public class DotnetInstallationFinalizerTests
{
    [Fact]
    public async Task FinalizeAsyncConfiguresEnvironmentWithCallerCancellationToken()
    {
        using var directory = new TestDirectory();
        await File.WriteAllTextAsync(Path.Combine(directory.Path, "dotnet"), "executable");
        var environmentManager = new RecordingUEManager();
        var finalizer = CreateFinalizer(directory.Path, environmentManager);
        using var cancellationTokenSource = new CancellationTokenSource();

        await finalizer.FinalizeAsync(cancellationTokenSource.Token);

        Assert.True(environmentManager.Called);
        Assert.Equal(cancellationTokenSource.Token, environmentManager.CancellationToken);
    }

    [Fact]
    public async Task FinalizeAsyncRejectsInstallationWithoutDotnetExecutable()
    {
        using var directory = new TestDirectory();
        var environmentManager = new RecordingUEManager();
        var finalizer = CreateFinalizer(directory.Path, environmentManager);

        var exception = await Assert.ThrowsAsync<InstalledDotnetExecutableNotFoundException>(() =>
            finalizer.FinalizeAsync(CancellationToken.None));

        Assert.Equal(directory.Path, exception.InstallRoot);
        Assert.False(environmentManager.Called);
    }

    private static DotnetInstallationFinalizer CreateFinalizer(string installRoot,
        IUEManager environmentManager)
    {
        return new DotnetInstallationFinalizer(
            new OrchestratorStubPathProvider(installRoot),
            NullLogger<DotnetInstallationFinalizer>.Instance,
            environmentManager);
    }

    private sealed class RecordingUEManager : IUEManager
    {
        public bool Called { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        public Task ConfigureEnvironment(CancellationToken cancellationToken)
        {
            Called = true;
            CancellationToken = cancellationToken;
            return Task.CompletedTask;
        }
    }
}
