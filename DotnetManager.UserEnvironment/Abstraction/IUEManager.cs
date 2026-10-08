namespace DotnetManager.UserEnvironment.Abstraction;

public interface IUEManager
{
    Task ConfigureEnvironment(CancellationToken cancellationToken);
}