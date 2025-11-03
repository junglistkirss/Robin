using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class CharDataFacade(char value) : IDataFacade
{
    public object? RawValue => value;

    public bool IsCollection() => false;
    public bool IsTrue() => value is not '\0';
    public bool IsCollection([NotNullWhen(true)] out IEnumerator? collection)
    {
        collection = null;
        return false;
    }
}
