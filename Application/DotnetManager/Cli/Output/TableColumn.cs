namespace DotnetManager.Cli.Output;

public sealed record TableColumn<T>(string Header, Func<T, string> Value);