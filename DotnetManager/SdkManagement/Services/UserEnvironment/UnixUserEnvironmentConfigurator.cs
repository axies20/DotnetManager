using DotnetManager.SdkManagement.Abstractions.UserEnvironment;

namespace DotnetManager.SdkManagement.Services.UserEnvironment;

public class UnixUserEnvironmentConfigurator : IUserEnvironmentConfigurator
{
    private static readonly HashSet<string> SupportedShells =
    [
        "bash",
        "zsh",
        "fish"
    ];

    public async Task ConfigureAsync(string dotnetRoot, CancellationToken cancellationToken = default)
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
        const string startMarker = "# DotnetManager";
        const string endMarker = "# /DotnetManager";

        var block = $"""
                     {startMarker}
                     {configuration}
                     {endMarker}
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

        var start = content.IndexOf(startMarker, StringComparison.Ordinal);
        var end = content.IndexOf(endMarker, StringComparison.Ordinal);

        if (start >= 0 && end > start)
        {
            end += endMarker.Length;

            content = string.Concat(content.AsSpan(0, start), block, content.AsSpan(end));

            await File.WriteAllTextAsync(path, content, cancellationToken);

            return;
        }

        await File.AppendAllTextAsync(path, Environment.NewLine + block + Environment.NewLine, cancellationToken);
    }
}