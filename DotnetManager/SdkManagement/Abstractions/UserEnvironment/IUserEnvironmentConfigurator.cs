namespace DotnetManager.SdkManagement.Abstractions.UserEnvironment;

public interface IUserEnvironmentConfigurator
{
    Task ConfigureAsync(string dotnetRoot, CancellationToken cancellationToken);
}