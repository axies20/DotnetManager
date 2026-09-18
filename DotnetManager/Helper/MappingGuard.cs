using System.Runtime.CompilerServices;

namespace DotnetManager.Helper;

public static class MappingGuard
{
    public static T Required<T>(T? value,
        [CallerArgumentExpression(nameof(value))]
        string? propertyName = null) where T : struct
    {
        return value ?? ThrowMissing<T>(propertyName);
    }

    public static T Required<T>(T? value,
        [CallerArgumentExpression(nameof(value))]
        string? propertyName = null)
    {
        return value is not null
            ? value
            : ThrowMissing<T>(propertyName);
    }

    public static string Required(string? value,
        [CallerArgumentExpression(nameof(value))]
        string? propertyName = null)

    {
        return !string.IsNullOrWhiteSpace(value)
            ? value
            : ThrowMissing<string>(propertyName);
    }


    private static T ThrowMissing<T>(string? propertyName)
    {
        throw new InvalidDataException(
            $"Required property '{propertyName}' is missing.");
    }
}