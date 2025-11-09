using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class BooleanDataFacade : IDataFacade
{
    public readonly static BooleanDataFacade Instance = new();
    private BooleanDataFacade() { }

    public bool IsTrue(object? obj) => obj is bool b && b;
    public bool IsCollection(object? _, out IIterator? collection)
    {
        collection = null;
        return false;
    }
}