namespace DotnetManager.InstalledDotnet.Abstractions;

public interface IDotnetInstallationLocator<out TInstallation>
{
    IReadOnlyCollection<TInstallation> Find();
}