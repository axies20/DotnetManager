namespace DotnetManager.UnitTests;

internal sealed class TestDirectory : IDisposable
{

    public string Path { get; }

    public TestDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"dotnet-manager-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path);
    }

    public string CreateDirectory(params string[] parts)
    {
        var path = parts.Aggregate(Path, System.IO.Path.Combine);
        Directory.CreateDirectory(path);
        return path;
    }

    public void Dispose()
    {
        Directory.Delete(Path, true);
    }
}