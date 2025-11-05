using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class ListDataFacade : IDataFacade
{
    public readonly static ListDataFacade Instance = new();
    private ListDataFacade() { }
    public bool IsTrue(object? obj) => obj is IList list && list.Count > 0;
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IEnumerable? collection)
    {
        if (obj is IList list)
        {
            collection = list;
            return list.Count > 0;
        }
        collection = null;
        return false;
    }
}
