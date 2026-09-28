namespace DotnetManager.Installation.Exceptions;

public sealed class InstalledDotnetExecutableNotFoundException(string installRoot)
    : DotnetInstallException(
        $"The extracted .NET files in '{installRoot}' do not contain the dotnet executable.")
{
    public string InstallRoot { get; } = installRoot;
}