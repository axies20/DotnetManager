using DotnetManager.Exception.Installation;
using DotnetManager.SdkManagement.Abstractions.Installation;
using DotnetManager.SdkManagement.Abstractions.InstallPaths;
using DotnetManager.SdkManagement.Abstractions.UserEnvironment;

namespace DotnetManager.SdkManagement.Services.Installation;

public class DotnetInstallationFinalizer : IDotnetInstallationFinalizerService
{
    private readonly IDotnetInstallPathProviderService _pathProvider;
    private readonly IUserEnvironmentConfiguratorService _userEnvironmentConfigurator;

    public DotnetInstallationFinalizer(IDotnetInstallPathProviderService pathProvider,
        IUserEnvironmentConfiguratorService userEnvironmentConfigurator)
    {
        _pathProvider = pathProvider;
        _userEnvironmentConfigurator = userEnvironmentConfigurator;
    }

    public async Task FinalizeAsync(CancellationToken cancellationToken)
    {
        var installRoot = _pathProvider.GetInstallDirectory();
        var executable = Path.Combine(installRoot, "dotnet");

        if (!File.Exists(executable))
            throw new InstalledDotnetExecutableNotFoundException(installRoot);

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
