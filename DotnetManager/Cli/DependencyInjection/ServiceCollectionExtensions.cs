using DotnetManager.Cli.Abstractions;
using DotnetManager.Cli.Commands.Available;
using DotnetManager.Cli.Commands.Available.Abstraction;
using DotnetManager.Cli.Commands.Available.Services;
using DotnetManager.Cli.Commands.Install;
using DotnetManager.Cli.Commands.List;
using DotnetManager.Cli.Commands.Remove;
using DotnetManager.Cli.Commands.Update;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetManager.Cli.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCli(this IServiceCollection services)
    {
        services.AddSingleton<ISdkReleaseService, SdkReleaseService>();
        services.AddTransient<ICommand, AvailableCommand>();
        services.AddTransient<ICommand, InstallCommand>();
        services.AddTransient<ICommand, ListCommand>();
        services.AddTransient<ICommand, RemoveCommand>();
        services.AddTransient<ICommand, UpdateCommand>();
        services.AddSingleton<CommandRegistration>();

        return services;
    }
}
