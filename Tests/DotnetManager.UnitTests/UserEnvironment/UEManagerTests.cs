using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;
using DotnetManager.UserEnvironment.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotnetManager.UnitTests.UserEnvironment;

public class UEManagerTests
{
    [Fact]
    public async Task ConfigureEnvironmentConfiguresEveryResolvedTargetAndPropagatesCancellation()
    {
        var fish = new RecordingConfigurator(UEKind.Fish);
        var environmentD = new RecordingConfigurator(UEKind.EnvironmentD);
        var resolver = new StubResolver(
        [
            new UEResolvedConfiguration(fish, "/config/fish"),
            new UEResolvedConfiguration(environmentD, "/config/environment.d")
        ]);
        var manager = new UEManager(resolver, NullLogger<UEManager>.Instance);
        using var cancellationTokenSource = new CancellationTokenSource();

        await manager.ConfigureEnvironment(cancellationTokenSource.Token);

        Assert.Equal(("/config/fish", cancellationTokenSource.Token), fish.Call);
        Assert.Equal(("/config/environment.d", cancellationTokenSource.Token), environmentD.Call);
    }

    private sealed class StubResolver(IReadOnlyCollection<UEResolvedConfiguration> configurations)
        : IUEResolver
    {
        public IReadOnlyCollection<UEResolvedConfiguration> Resolve()
        {
            return configurations;
        }
    }

    private sealed class RecordingConfigurator(UEKind kind) : IUEConfigurator
    {
        public UEKind Kind { get; } = kind;
        public (string Path, CancellationToken CancellationToken)? Call { get; private set; }

        public Task ConfigureAsync(string path, CancellationToken cancellationToken)
        {
            Call = (path, cancellationToken);
            return Task.CompletedTask;
        }

        public void Remove(string path)
        {
        }
    }
}
