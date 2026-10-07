using DotnetManager.UserEnvironment.Abstraction;
using DotnetManager.UserEnvironment.Models;

namespace DotnetManager.UserEnvironment.Services.Configuration;

internal abstract class UEConfiguratorBase : IUEConfigurator
{
    public abstract UEKind Kind { get; }
    protected abstract string FileName { get; }

    public async Task ConfigureAsync(string path, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(path);
        var content = BuildContent();

        if (File.Exists(FileName))
        {
            var currentContent = await File.ReadAllTextAsync(FileName, cancellationToken);

            if (currentContent == content)
                return;
        }

        await File.WriteAllTextAsync(FileName, content, cancellationToken);
    }

    public void Remove(string path)
    {
        var fileName = Path.Combine(path, FileName);
        File.Delete(fileName);
    }

    protected abstract string BuildContent();
}
