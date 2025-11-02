using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class ShortDataFacade(short value) : IDataFacade
{
    public object? RawValue => value;

    public bool IsCollection() => false;
    public bool IsTrue() => value > 0;
    public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
    {
        collection = null;
        return false;
    }
}