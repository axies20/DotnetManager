using System.CommandLine;
using NuGet.Versioning;

namespace DotnetManager.Cli.Commands.Available;

internal sealed partial class AvailableCommand
{
    private readonly Argument<NuGetVersion?> _channelArgument = new("channel")
    {
        Description = "The .NET channel to show available SDKs for.",
        Arity = ArgumentArity.ZeroOrOne,
        CustomParser = result =>
        {
            var value = result.Tokens.Single().Value;

            if (NuGetVersion.TryParse(value, out var version))
                return version;

            result.AddError($"'{value}' is not a valid .NET channel version.");
            return null;
        }
    };
}