using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class IListIterator(IList list) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        foreach (object? item in list)
            action(item);
    }
}

internal sealed class IListDataFacade : IDataFacade
{
    public readonly static IListDataFacade Instance = new();
    private IListDataFacade() { }
    public bool IsTrue(object? obj) => obj is IList list && list.Count > 0;
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection)
    {
        if (obj is IList list)
        {
            collection = new IListIterator(list);
            return list.Count > 0;
        }
        collection = null;
        return false;
    }
}
