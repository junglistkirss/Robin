using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class NullDataFacade : IDataFacade
{
    public object? RawValue => null;

    public bool IsCollection() => false;

    public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
    {
        collection = null;
        return false;
    }

    public bool IsTrue() => false;
}
