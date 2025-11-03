using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class NumericDataFacade<T>(T value) : IDataFacade
    where T : unmanaged
{
    public object? RawValue => value;

    public bool IsCollection() => false;
    public bool IsTrue() => value is float f ? f is not float.NaN : value is double d ? d is not double.NaN : true;
    public bool IsCollection([NotNullWhen(true)] out IEnumerator? collection)
    {
        collection = null;
        return false;
    }
}

