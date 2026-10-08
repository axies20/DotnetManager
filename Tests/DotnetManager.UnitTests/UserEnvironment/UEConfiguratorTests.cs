using DotnetManager.Core.Models;
using DotnetManager.UserEnvironment.Models;
using DotnetManager.UserEnvironment.Services.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotnetManager.UnitTests.UserEnvironment;

public class UEConfiguratorTests
{
    [Theory]
    [InlineData(UEKind.Fish, "dnm.fish")]
    [InlineData(UEKind.OhMyZsh, "60-dnm.zsh")]
    [InlineData(UEKind.EnvironmentD, "60-dnm.conf")]
    public async Task ConfigureAsyncWritesDotnetPaths(UEKind kind, string fileName)
    {
        using var directory = new TestDirectory();
        var configurator = CreateConfigurator(kind);
        var filePath = Path.Combine(directory.Path, fileName);
        await File.WriteAllTextAsync(filePath, "stale content");

        await configurator.ConfigureAsync(directory.Path, CancellationToken.None);

        var content = await File.ReadAllTextAsync(filePath);
        Assert.Contains(DotnetPaths.UserInstallRoot, content, StringComparison.Ordinal);
        Assert.Contains(DotnetPaths.UserToolsRoot, content, StringComparison.Ordinal);
        Assert.DoesNotContain("stale content", content, StringComparison.Ordinal);
    }

    private static UEConfiguratorBase CreateConfigurator(UEKind kind)
    {
        return kind switch
        {
            UEKind.Fish => new FishConfig(NullLogger<FishConfig>.Instance),
            UEKind.OhMyZsh => new OhMyZshUEConfig(NullLogger<OhMyZshUEConfig>.Instance),
            UEKind.EnvironmentD => new EnvironmentDUEConfig(NullLogger<EnvironmentDUEConfig>.Instance),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }
}
