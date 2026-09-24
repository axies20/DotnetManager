using System.Diagnostics;
using Xunit;

namespace DotnetManager.IntegrationTests;

public class InstallContainerTests
{
    [PodmanFact]
    [Trait("Category", "Podman")]
    public async Task LatestSdkIsInstalledInsideFreshContainer()
    {
        var repositoryRoot = FindRepositoryRoot();
        var containerName = $"dotnet-manager-install-test-{Guid.NewGuid():N}";

        try
        {
            await RunAsync("podman", "create", "--name", containerName,
                "mcr.microsoft.com/dotnet/sdk:10.0", "sleep", "infinity");
            await RunAsync("podman", "start", containerName);
            await RunAsync("podman", "cp", $"{repositoryRoot}/.", $"{containerName}:/src");
            await RunAsync("podman", "exec", containerName, "dotnet", "run",
                "--project", "/src/DotnetManager/DotnetManager.csproj",
                "-p:PublishAot=false", "--", "install", "latest", "--rid", "linux-x64");
            await RunAsync("podman", "exec", containerName, "test", "-x",
                "/usr/local/share/dotnet/dotnet");
            var info = await RunAsync("podman", "exec", containerName,
                "/usr/local/share/dotnet/dotnet", "--info");

            Assert.Contains(".NET SDK", info, StringComparison.Ordinal);
        }
        finally
        {
            await RunAsync("podman", ["rm", "--force", containerName], false);
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null &&
               !File.Exists(Path.Combine(directory.FullName,
                   "DotnetManager.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ??
               throw new InvalidOperationException("Could not find the repository root.");
    }

    private static Task<string> RunAsync(string fileName, params string[] arguments)
    {
        return RunAsync(fileName, arguments, true);
    }

    private static async Task<string> RunAsync(string fileName, string[] arguments, bool throwOnError)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        foreach (var argument in arguments)
            process.StartInfo.ArgumentList.Add(argument);

        process.Start();
        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        var output = await standardOutput;
        var error = await standardError;

        if (throwOnError && process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"{fileName} {string.Join(' ', arguments)} failed with exit code " +
                $"{process.ExitCode}:{Environment.NewLine}{output}{Environment.NewLine}{error}");
        }

        return output + error;
    }
}