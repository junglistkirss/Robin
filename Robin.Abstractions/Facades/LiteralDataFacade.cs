using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class LiteralDataFacade(string value) : IDataFacade
{
    public object? RawValue => value;

    public bool IsCollection() => false;
    public bool IsTrue() => !string.IsNullOrEmpty(value);
    public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
    {
        collection = null;
        return false;
    }
}
