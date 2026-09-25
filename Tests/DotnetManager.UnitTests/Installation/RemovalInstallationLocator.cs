using DotnetManager.InstalledDotnet.Abstractions;

namespace DotnetManager.UnitTests.Installation;

internal sealed class RemovalInstallationLocator<TInstallation>(
    IReadOnlyCollection<TInstallation> installations,
    Func<TInstallation, string> pathSelector)
    : IDotnetInstallationLocatorService<TInstallation>
{
    public IReadOnlyCollection<TInstallation> Find()
    {
        return installations.Where(x => Directory.Exists(pathSelector(x))).ToList();
    }
}
