using Robin.Abstractions.Iterators;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class IListDataFacade : IDataFacade
{
    public readonly static IListDataFacade Instance = new();
    private IListDataFacade() { }
    public bool IsTrue(object? obj) => obj is IList list && list.Count > 0;
    public bool IsCollection(object? obj, out IIterator? collection)
    {
        return IteratorCache.GetIterator(obj, out collection);
    }
}
