using DotnetManager.UserEnvironment.Abstractions;
using Microsoft.Extensions.Logging;

namespace DotnetManager.UserEnvironment.Services;

public class UnixUserEnvironmentConfigurator : IUserEnvironmentConfiguratorService
{
    private readonly ILogger<UnixUserEnvironmentConfigurator> _logger;

    public UnixUserEnvironmentConfigurator(ILogger<UnixUserEnvironmentConfigurator> logger)
    {
        _logger = logger;
    }

    public async Task ConfigureAsync(string dotnetRoot, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Inspecting shell configuration for .NET root {DotnetRoot}",
            dotnetRoot);
        var shells = GetInstalledShells().ToHashSet();
        _logger.LogDebug("Found installed shells: {Shells}", string.Join(", ", shells));
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _logger.LogDebug("User home directory is {HomeDirectory}", home);

        if (shells.Contains("bash"))
        {
            var bashrcPath = Path.Combine(home, ".bashrc");
            await ConfigurePosixShellAsync(bashrcPath, dotnetRoot, cancellationToken);
        }

        if (shells.Contains("zsh"))
        {
            var zshrcPath = Path.Combine(home, ".zshrc");
            await ConfigurePosixShellAsync(zshrcPath, dotnetRoot, cancellationToken);
        }

        if (shells.Contains("fish"))
        {
            await ConfigureFishAsync(dotnetRoot, cancellationToken);
        }

        _logger.LogInformation("Shell configuration inspection completed");
    }

    private IEnumerable<string?> GetInstalledShells()
    {
        if (!File.Exists("/etc/shells"))
        {
            _logger.LogWarning("Cannot discover installed shells because {ShellsFile} does not exist",
                "/etc/shells");
            yield break;
        }

        var shells = File.ReadLines("/etc/shells")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Where(x => !x.StartsWith('#'))
            .Select(Path.GetFileName)
            .Where(x => x is not null)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var line in shells)
        {
            _logger.LogTrace("Discovered shell {Shell}", line);
            yield return line;
        }
    }

    private Task ConfigurePosixShellAsync(string path, string dotnetRoot, CancellationToken cancellationToken)
    {
        var configuration = $"""
                             export DOTNET_ROOT="{dotnetRoot}"
                             export PATH="$DOTNET_ROOT:$PATH"
                             """;

        return ConfigureShellAsync(
            path,
            configuration,
            cancellationToken);
    }

    private Task ConfigureFishAsync(string dotnetRoot, CancellationToken cancellationToken)
    {
        var home = Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile);

        var path = Path.Combine(home, ".config", "fish", "config.fish");

        var configuration = $"""
                             set -gx DOTNET_ROOT "{dotnetRoot}"
                             fish_add_path $DOTNET_ROOT
                             """;

        return ConfigureShellAsync(path, configuration, cancellationToken);
    }

    private async Task ConfigureShellAsync(string path,
        string configuration,
        CancellationToken cancellationToken)
    {
        const string startMarker = "# DotnetManager";
        const string endMarker = "# /DotnetManager";

        var block = $"""
                     {startMarker}
                     {configuration}
                     {endMarker}
                     """;

        var directory = Path.GetDirectoryName(path);

        if (directory is not null)
        {
            _logger.LogDebug("Ensuring shell configuration directory {Directory}", directory);
            Directory.CreateDirectory(directory);
        }

        _logger.LogInformation("Checking shell configuration file {Path}", path);

        if (!File.Exists(path))
        {
            await File.WriteAllTextAsync(path, block + Environment.NewLine, cancellationToken);
            _logger.LogInformation("Created shell configuration file {Path}", path);
            return;
        }

        var content = await File.ReadAllTextAsync(path, cancellationToken);

        if (content.Contains(configuration, StringComparison.Ordinal))
        {
            _logger.LogDebug("Shell configuration file {Path} is already up to date", path);
            return;
        }

        var start = content.IndexOf(startMarker, StringComparison.Ordinal);
        var end = content.IndexOf(endMarker, StringComparison.Ordinal);

        if (start >= 0 && end > start)
        {
            end += endMarker.Length;

            content = string.Concat(content.AsSpan(0, start), block, content.AsSpan(end));

            await File.WriteAllTextAsync(path, content, cancellationToken);
            _logger.LogInformation("Updated managed block in {Path}", path);

            return;
        }

        await File.AppendAllTextAsync(path, Environment.NewLine + block + Environment.NewLine, cancellationToken);
        _logger.LogInformation("Added managed block to {Path}", path);
    }
}