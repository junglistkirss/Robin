using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class ObjectDataFacade(object? value) : IDataFacade
{
    public object? RawValue => value;

    public bool IsCollection() => value is IEnumerable;
    public bool IsTrue() => value is not null;
    public bool IsCollection([NotNullWhen(true)] out IEnumerator? collection)
    {
        collection = value as IEnumerator;
        return collection is not null;
    }
}
