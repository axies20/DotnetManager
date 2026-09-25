using DotnetManager.Installation.Abstractions.UserEnvironment;

namespace DotnetManager.Installation.Services.UserEnvironment;

public class UnixUserEnvironmentConfigurator : IUserEnvironmentConfiguratorService
{
    private const string StartMarker = "# DotnetManager";
    private const string EndMarker = "# /DotnetManager";

    public async Task ConfigureAsync(string dotnetRoot, CancellationToken cancellationToken)
    {
        var shells = GetInstalledShells().ToHashSet();
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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
            await ConfigureFishAsync(dotnetRoot, cancellationToken);
    }

    public async Task RemoveConfigurationAsync(string dotnetRoot, CancellationToken cancellationToken)
    {
        var shells = GetInstalledShells().ToHashSet();
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (shells.Contains("bash"))
            await RemoveShellConfigurationAsync(Path.Combine(home, ".bashrc"), dotnetRoot, cancellationToken);

        if (shells.Contains("zsh"))
            await RemoveShellConfigurationAsync(Path.Combine(home, ".zshrc"), dotnetRoot, cancellationToken);

        if (shells.Contains("fish"))
        {
            await RemoveShellConfigurationAsync(
                Path.Combine(home, ".config", "fish", "config.fish"),
                dotnetRoot,
                cancellationToken);
        }
    }

    private static IEnumerable<string?> GetInstalledShells()
    {
        if (!File.Exists("/etc/shells"))
            yield break;

        var shells = File.ReadLines("/etc/shells")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Where(x => !x.StartsWith('#'))
            .Select(Path.GetFileName)
            .Where(x => x is not null)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var line in shells)
        {
            yield return line;
        }
    }

    private static Task ConfigurePosixShellAsync(string path, string dotnetRoot, CancellationToken cancellationToken)
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

    private static Task ConfigureFishAsync(string dotnetRoot, CancellationToken cancellationToken)
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

    private static async Task ConfigureShellAsync(string path,
        string configuration,
        CancellationToken cancellationToken)
    {
        var block = $"""
                     {StartMarker}
                     {configuration}
                     {EndMarker}
                     """;

        var directory = Path.GetDirectoryName(path);

        if (directory is not null)
            Directory.CreateDirectory(directory);

        if (!File.Exists(path))
        {
            await File.WriteAllTextAsync(path, block + Environment.NewLine, cancellationToken);

            return;
        }

        var content = await File.ReadAllTextAsync(path, cancellationToken);

        if (content.Contains(configuration, StringComparison.Ordinal))
            return;

        var start = content.IndexOf(StartMarker, StringComparison.Ordinal);
        var end = content.IndexOf(EndMarker, StringComparison.Ordinal);

        if (start >= 0 && end > start)
        {
            end += EndMarker.Length;

            content = string.Concat(content.AsSpan(0, start), block, content.AsSpan(end));

            await File.WriteAllTextAsync(path, content, cancellationToken);

            return;
        }

        await File.AppendAllTextAsync(path, Environment.NewLine + block + Environment.NewLine, cancellationToken);
    }

    private static async Task RemoveShellConfigurationAsync(
        string path,
        string dotnetRoot,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
            return;

        var content = await File.ReadAllTextAsync(path, cancellationToken);
        var start = content.IndexOf(StartMarker, StringComparison.Ordinal);
        var end = content.IndexOf(EndMarker, StringComparison.Ordinal);

        if (start < 0 || end <= start)
            return;

        end += EndMarker.Length;
        var managedBlock = content[start..end];

        if (!managedBlock.Contains($"\"{dotnetRoot}\"", StringComparison.Ordinal))
            return;

        var removeStart = start;
        var removeEnd = end;

        if (removeStart > 0 && content[removeStart - 1] == '\n')
            removeStart--;

        if (removeEnd < content.Length && content[removeEnd] == '\r')
            removeEnd++;

        if (removeEnd < content.Length && content[removeEnd] == '\n')
            removeEnd++;

        var updated = string.Concat(content.AsSpan(0, removeStart), content.AsSpan(removeEnd));
        await File.WriteAllTextAsync(path, updated, cancellationToken);
    }
}
