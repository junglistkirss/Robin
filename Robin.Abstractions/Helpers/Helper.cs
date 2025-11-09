using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Helpers;

public sealed class Helper
{
    public delegate object? Function(object?[] args);

    private readonly Dictionary<string, Function> _functions = [];

    public bool TryAddFunction(string name, Function function)
    {
        return _functions.TryAdd(name, function);
    }

    public bool TryGetFunction(string name, [MaybeNullWhen(false)] out Function? function)
    {
        return _functions.TryGetValue(name, out function);
    }
}
