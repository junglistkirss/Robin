using Robin.Abstractions.Iterators;

namespace Robin.Abstractions.Facades;

internal sealed class ObjectDataFacade : IDataFacade
{
    public readonly static ObjectDataFacade Instance = new();
    private ObjectDataFacade() { }
    public bool IsTrue(object? value) => value is not null;
    public bool IsCollection(object? value, out IIterator? collection)
    {
        return IteratorCache.GetIterator(value, out collection);
    }
}
