using DotnetManager.SdkManagement.Abstractions.Installation;
using DotnetManager.SdkManagement.Abstractions.InstallPaths;
using DotnetManager.SdkManagement.Abstractions.UserEnvironment;

namespace DotnetManager.SdkManagement.Services.Installation;

public class DotnetInstaller : IDotnetInstaller
{
    private readonly IDotnetInstallPathProvider _pathProvider;
    private readonly IUserEnvironmentConfigurator _userEnvironmentConfigurator;

    public DotnetInstaller(IDotnetInstallPathProvider pathProvider,
        IUserEnvironmentConfigurator userEnvironmentConfigurator)
    {
        _pathProvider = pathProvider;
        _userEnvironmentConfigurator = userEnvironmentConfigurator;
    }

    public async Task InstallAsync(CancellationToken cancellationToken = default)
    {
        var installRoot = _pathProvider.GetInstallDirectory();
        var executable = Path.Combine(installRoot, "dotnet");

        if (!File.Exists(executable))
            throw new InvalidDataException(
                "The extracted .NET archive does not contain the dotnet executable.");

        var linkPath = _pathProvider.GetExecutableLinkPath();

        if (linkPath is not null)
        {
            EnsureExecutableLink(linkPath, executable);
            return;
        }

        await _userEnvironmentConfigurator.ConfigureAsync(installRoot, cancellationToken);
    }

    private static void EnsureExecutableLink(string linkPath, string targetPath)
    {
        var link = new FileInfo(linkPath);

        if (link.Exists || link.LinkTarget is not null)
            return;

        File.CreateSymbolicLink(linkPath, targetPath);
    }
}
