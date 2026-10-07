using System.CommandLine;
using DotnetManager.Cli;
using DotnetManager.DependencyInjection;
using DotnetManager.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DotnetManager;

internal abstract class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (UnixHelper.IsRoot())
        {
            Console.Error.WriteLine("DotnetManager must not be run as root. Run it as a regular user.");
            return 1;
        }

        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            ContentRootPath = AppContext.BaseDirectory
        });
        builder.Logging.AddSimpleConsole(options =>
        {
            options.ColorBehavior = Console.IsErrorRedirected
                ? LoggerColorBehavior.Disabled
                : LoggerColorBehavior.Enabled;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });
        builder.Services.Configure<ConsoleLoggerOptions>(options =>
            options.LogToStandardErrorThreshold = LogLevel.Trace);
        builder.Services.AddDotnetManager();

        using var host = builder.Build();
        var rootCommand = new RootCommand("Discover, install, update, and remove .NET SDKs from multiple " +
                                          "release channels, while managing tracked channels and pinned SDK versions from a " +
                                          "single command-line interface.");

        var commandRegistration = host.Services.GetRequiredService<CommandRegistration>();
        commandRegistration.AddSubCommands(rootCommand);

        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        try
        {
            return await rootCommand.Parse(args).InvokeAsync();
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Command execution was canceled");
            return 130;
        }
        catch (global::System.Exception exception)
        {
            logger.LogError(exception, "Command execution failed");
            return 1;
        }
    }
}