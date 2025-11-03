using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class ListDataFacade(IList value) : IDataFacade
{
    public object? RawValue => value;

    public bool IsCollection() => true;
    public bool IsTrue() => value.Count > 0;
    public bool IsCollection([NotNullWhen(true)] out IEnumerator? collection)
    {
        collection = value.GetEnumerator();
        return value.Count > 0;
    }
}
