using Robin.Abstractions.Iterators;

namespace Robin.Abstractions.Facades;

internal sealed class NullDataFacade : IDataFacade
{
    public readonly static NullDataFacade Instance = new();
    private NullDataFacade() { }
    public bool IsCollection(object? _, out IIterator? collection)
    {
        collection = null;
        return false;
    }

    public bool IsTrue(object? _) => false;
}
