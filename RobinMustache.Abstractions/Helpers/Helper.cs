using static RobinMustache.Abstractions.Helpers.Helper;

namespace RobinMustache.Abstractions.Helpers;

public static class GlobalHelpers
{
    private static readonly Dictionary<string, Function> _functions = new(StringComparer.OrdinalIgnoreCase);

    public static bool TryAddFunction(string name, Function function)
    {
        if (_functions.ContainsKey(name))
            return false;
        _functions.Add(name, function);
        return true;
    }

    public static bool TryGetFunction(string name, out Function? function)
    {
        return _functions.TryGetValue(name, out function);
    }
}

public sealed class Helper
{
    public delegate object? Function(object?[] args);

    private readonly Dictionary<string, Function> _functions = new(StringComparer.OrdinalIgnoreCase);

    public bool TryAddFunction(string name, Function function)
    {
        if(_functions.ContainsKey(name))
            return false;
        _functions.Add(name, function);
        return true;
    }

    public bool TryGetFunction(string name, out Function? function)
    {
        return _functions.TryGetValue(name, out function);
    }
}
