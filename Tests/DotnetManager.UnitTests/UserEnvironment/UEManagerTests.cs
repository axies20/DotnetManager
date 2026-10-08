using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;
using DotnetManager.UserEnvironment.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotnetManager.UnitTests.UserEnvironment;

public class UEManagerTests
{
    [Fact]
    public void UnixManualConfigurationIncludesCopyableZshAndBashInstructions()
    {
        var instructions = UEManager.UnixManualShellConfigurationInstructions;

        Assert.Contains("Zsh - add to ~/.zshrc:", instructions);
        Assert.Contains("Bash - add to ~/.bashrc:", instructions);
        Assert.Equal(2, CountOccurrences(instructions, "export DOTNET_ROOT=\"$HOME/.dotnet\""));
        Assert.Equal(2, CountOccurrences(instructions,
            "export PATH=\"$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH\""));
        Assert.Contains("source ~/.zshrc", instructions);
        Assert.Contains("source ~/.bashrc", instructions);
    }

    [Fact]
    public void EnvironmentDWithoutShellConfigurationRequiresManualShellInstructions()
    {
        var environmentD = new RecordingConfigurator(UEKind.EnvironmentD);
        IReadOnlyCollection<UEResolvedConfiguration> configurations =
        [new UEResolvedConfiguration(environmentD, "/config/environment.d")];

        Assert.True(UEManager.RequiresManualShellConfiguration(configurations));
    }

    [Theory]
    [InlineData(UEKind.Fish)]
    [InlineData(UEKind.OhMyZsh)]
    public void ResolvedShellConfigurationDoesNotRequireManualInstructions(UEKind kind)
    {
        var shell = new RecordingConfigurator(kind);
        IReadOnlyCollection<UEResolvedConfiguration> configurations =
        [new UEResolvedConfiguration(shell, "/config/shell")];

        Assert.False(UEManager.RequiresManualShellConfiguration(configurations));
    }

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

        public void Remove(string path) {}
    }

    private static int CountOccurrences(string value, string searchValue)
    {
        return value.Split(searchValue, StringSplitOptions.None).Length - 1;
    }
}
