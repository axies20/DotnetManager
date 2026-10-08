using DotnetManager.UserEnvironment.Abstraction;

namespace DotnetManager.UserEnvironment.Models;

internal sealed record UEResolvedConfiguration(IUEConfigurator Configurator, string Path);