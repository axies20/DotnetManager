using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;
using Microsoft.Extensions.Logging;

namespace DotnetManager.UserEnvironment.Services.Configuration;

internal abstract class UEConfiguratorBase(ILogger logger) : IUEConfigurator
{
    public abstract UEKind Kind { get; }
    protected abstract string FileName { get; }

    public async Task ConfigureAsync(string path, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(path);
        var fileName = Path.Combine(path, FileName);
        var content = BuildContent();

        if (File.Exists(fileName))
        {
            var currentContent = await File.ReadAllTextAsync(fileName, cancellationToken);

            if (currentContent == content)
            {
                logger.LogInformation(
                    "{Environment} environment configuration is already up to date at {FilePath}",
                    Kind, fileName);
                return;
            }
        }

        await File.WriteAllTextAsync(fileName, content, cancellationToken);
        logger.LogInformation("Configured {Environment} environment at {FilePath}",
            Kind, fileName);
    }

    public void Remove(string path)
    {
        var fileName = Path.Combine(path, FileName);

        if (!File.Exists(fileName))
        {
            logger.LogInformation(
                "No {Environment} environment configuration was found at {FilePath}",
                Kind, fileName);
            return;
        }

        File.Delete(fileName);
        logger.LogInformation("Removed {Environment} environment configuration from {FilePath}",
            Kind, fileName);
    }

    protected abstract string BuildContent();
}