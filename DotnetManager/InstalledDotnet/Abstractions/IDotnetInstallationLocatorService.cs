namespace DotnetManager.InstalledDotnet.Abstractions;

public interface IDotnetInstallationLocatorService<out TInstallation>
{
    IReadOnlyCollection<TInstallation> Find();
}
