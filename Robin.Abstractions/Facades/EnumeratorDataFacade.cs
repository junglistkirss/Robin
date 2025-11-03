using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class EnumeratorDataFacade(IEnumerator value) : IDataFacade
{
    public object? RawValue => value;

    public bool IsCollection() => true;
    public bool IsTrue() => true;
    public bool IsCollection([NotNullWhen(true)] out IEnumerator? collection)
    {
        collection = value;
        return true;
    }
}
