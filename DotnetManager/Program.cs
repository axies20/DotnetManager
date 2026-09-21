using System.CommandLine;
using DotnetManager.Cli;
using DotnetManager.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotnetManager;

internal abstract class Program
{
    private static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            ContentRootPath = AppContext.BaseDirectory
        });
        builder.Services.AddDotnetManager();

        using var host = builder.Build();
        var rootCommand = new RootCommand("Discover, install, update, and remove .NET SDKs from multiple " +
                                          "release channels, while managing tracked channels and pinned SDK versions from a " +
                                          "single command-line interface.");

        var commandRegistration = host.Services.GetRequiredService<CommandRegistration>();
        commandRegistration.AddSubCommands(rootCommand);

        return await rootCommand.Parse(args).InvokeAsync();
    }
}