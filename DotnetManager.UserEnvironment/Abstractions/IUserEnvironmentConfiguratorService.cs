namespace DotnetManager.UserEnvironment.Abstractions;

public interface IUserEnvironmentConfiguratorService
{
    Task ConfigureAsync(string dotnetRoot, CancellationToken cancellationToken);
}