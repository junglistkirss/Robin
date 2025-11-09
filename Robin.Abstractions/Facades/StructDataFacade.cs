using Robin.Abstractions.Iterators;

namespace Robin.Abstractions.Facades;

internal sealed class StructDataFacade : IDataFacade
{
    public readonly static StructDataFacade Instance = new();
    private StructDataFacade() { }
    public bool IsTrue(object? obj) => obj is not null;
    public bool IsCollection(object? _, out IIterator? collection)
    {
        collection = null;
        return false;
    }
}