namespace DotnetManager.Installation.Abstractions.UserEnvironment;

public interface IUserEnvironmentConfiguratorService
{
    Task ConfigureAsync(string dotnetRoot, CancellationToken cancellationToken);
    Task RemoveConfigurationAsync(string dotnetRoot, CancellationToken cancellationToken);
}
