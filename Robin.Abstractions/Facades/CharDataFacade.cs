using Robin.Abstractions.Iterators;

namespace Robin.Abstractions.Facades;

internal sealed class CharDataFacade : IDataFacade
{
    public readonly static CharDataFacade Instance = new();
    private CharDataFacade() { }
    public bool IsTrue(object? obj) => obj is char c && c is not '\0';
    public bool IsCollection(object? _, out IIterator? collection)
    {
        collection = null;
        return false;
    }
}
