using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class EnumeratorDataFacade : IDataFacade
{
    public readonly static EnumeratorDataFacade Instance = new();
    private EnumeratorDataFacade() { }
    public bool IsTrue(object? _) => true;
    public static IEnumerable ToIEnumerable(IEnumerator enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IEnumerable? collection)
    {
        if (obj is IEnumerator value)
        {
            collection = ToIEnumerable(value);
            return true;
        }
        collection = null;
        return false;
    }
}
