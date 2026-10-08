using Spectre.Console;

namespace DotnetManager.Cli.Output;

public static class TablePrinter
{
    public static void Print<T>(string title, IEnumerable<T> items, params TableColumn<T>[] columns)
    {
        var table = ConfigureTable(title, columns);

        foreach (var item in items)
        {
            table.AddRow(columns.Select(column => Markup.Escape(column.Value(item))).ToArray());
        }

        AnsiConsole.Write(table);
    }

    public static void Print<T>(string title, T item, params TableColumn<T>[] columns)
    {
        var table = ConfigureTable(title, columns);

        table.AddRow(columns.Select(column => Markup.Escape(column.Value(item))).ToArray());

        AnsiConsole.Write(table);
    }

    private static Table ConfigureTable<T>(string title, params TableColumn<T>[] columns)
    {
        AnsiConsole.WriteLine();

        var table = new Table
        {
            Title = new TableTitle(Markup.Escape(title), new Style(decoration: Decoration.Bold)),
            Expand = true,
            ShowRowSeparators = true
        };

        foreach (var column in columns)
        {
            table.AddColumn(Markup.Escape(column.Header));
        }

        return table;
    }
}