using System.CommandLine;
using DotnetManager.Abstraction.SDK;
using DotnetManager.Cli;
using DotnetManager.Options;
using DotnetManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotnetManager;

internal abstract class Program
{
    private static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddOptions<DotnetManagerOptions>()
            .BindConfiguration("DotnetManager")
            .ValidateOnStart();
        builder.Services.AddHttpClient<ISdkDownloader, SdkDownloader>();

        var rootCommand = new RootCommand("Microsoft .NET SDK manager for Linux");
        rootCommand.Subcommands.Add(InstallCommand.Create());
        
        return await rootCommand.Parse(args).InvokeAsync();
    }
}