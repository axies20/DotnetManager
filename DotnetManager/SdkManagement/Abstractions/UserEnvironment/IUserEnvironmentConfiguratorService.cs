namespace DotnetManager.SdkManagement.Abstractions.UserEnvironment;

public interface IUserEnvironmentConfiguratorService
{
    Task ConfigureAsync(string dotnetRoot, CancellationToken cancellationToken);
}
