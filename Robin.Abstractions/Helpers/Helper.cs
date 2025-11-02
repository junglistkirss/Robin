using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Helpers;

public static class Helper
{
    public delegate object? Function(params object?[] args);

    private readonly static Dictionary<string, Function> _functions = new();

    public static bool TryAddFunction(string name, Function function)
    {
        return _functions.TryAdd(name, function);
    }

    public static bool TryGetFunction(string name, [MaybeNullWhen(false)] out Function? function)
    {
        return _functions.TryGetValue(name, out function);
    }
}
