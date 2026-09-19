using DotnetManager.Cli.Commands.List.Abstraction;
using DotnetManager.InstalledDotnet.Models;
using Spectre.Console;

namespace DotnetManager.Cli.Commands.List;

public sealed class SpectreListOutput : IListOutput
{
    public void PrintSdks(
        IReadOnlyCollection<SdkInstallation> sdks)
    {
        PrintTable(
            "Installed SDKs",
            sdks,
            ("Version", x => x.Version.ToString()),
            ("Path", x => x.Path));
    }

    public void PrintRuntimes(
        IReadOnlyCollection<RuntimeInstallation> runtimes)
    {
        PrintTable(
            "Installed Runtimes",
            runtimes,
            ("Framework", x => x.Framework),
            ("Version", x => x.Version.ToString()),
            ("Path", x => x.Path));
    }

    public void PrintHosts(
        IReadOnlyCollection<HostInstallation> hosts)
    {
        PrintTable(
            "Installed Hosts",
            hosts,
            ("Version", x => x.Version.ToString()),
            ("Path", x => x.Path));
    }

    private static void PrintTable<T>(
        string title,
        IReadOnlyCollection<T> items,
        params (string Header, Func<T, string> Value)[] columns)
    {
        AnsiConsole.WriteLine();

        var table = new Table
        {
            Title = new TableTitle(
                title,
                new Style(decoration: Decoration.Bold)),
            Expand = true
        };

        foreach (var column in columns)
            table.AddColumn(column.Header);

        foreach (var item in items)
        {
            table.AddRow(columns
                .Select(x => Markup.Escape(x.Value(item)))
                .ToArray());
        }

        AnsiConsole.Write(table);
    }
}