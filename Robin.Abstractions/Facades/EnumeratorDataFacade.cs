using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class IEnumeratorIterator(IEnumerator enumerator) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        while (enumerator.MoveNext()) 
            action(enumerator.Current);
    }
}

internal sealed class EnumeratorDataFacade : IDataFacade
{
    public readonly static EnumeratorDataFacade Instance = new();
    private EnumeratorDataFacade() { }
    public bool IsTrue(object? _) => true;
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection)
    {
        if (obj is IEnumerator value)
        {
            collection = new IEnumeratorIterator(value);
            return true;
        }
        collection = null;
        return false;
    }
}
