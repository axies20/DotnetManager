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
        if (!CanRun())
            return 1;

        using var host = CreateHost(args);
        var rootCommand = CreateRootCommand(host.Services);
        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        return await RunAsync(rootCommand, args, logger);
    }

    private static bool CanRun()
    {
        if (!UnixHelper.IsRoot())
            return true;

        Console.Error.WriteLine("DotnetManager must not be run as root. Run it as a regular user.");
        return false;
    }

    private static IHost CreateHost(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            ContentRootPath = AppContext.BaseDirectory
        });

        ConfigureLogging(builder);
        builder.Services.AddDotnetManager();

        return builder.Build();
    }

    private static void ConfigureLogging(HostApplicationBuilder builder)
    {
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
    }

    private static RootCommand CreateRootCommand(IServiceProvider services)
    {
        var rootCommand = new RootCommand(
            "Discover, install, list, and remove .NET SDKs and runtimes from a single command-line interface.");

        var commandRegistration = services.GetRequiredService<CommandRegistration>();
        commandRegistration.AddSubCommands(rootCommand);

        return rootCommand;
    }

    private static async Task<int> RunAsync(RootCommand rootCommand, string[] args, ILogger<Program> logger)
    {
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