using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class StructDataFacade<T>(T value) : IDataFacade
    where T : struct
{
    public object? RawValue => value;

    public bool IsCollection() => false;
    public bool IsTrue() => true;
    public bool IsCollection([NotNullWhen(true)] out IEnumerator? collection)
    {
        collection = null;
        return false;
    }
}