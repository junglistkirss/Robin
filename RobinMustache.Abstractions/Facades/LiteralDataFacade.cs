using RobinMustache.Abstractions.Iterators;

namespace RobinMustache.Abstractions.Facades;

internal sealed class LiteralDataFacade : IDataFacade
{
    public readonly static LiteralDataFacade Instance = new();
    private LiteralDataFacade() { }
    public bool IsTrue(object? obj) => obj is string value && !string.IsNullOrEmpty(value);
    public bool IsCollection(object? _, out IIterator? collection)
    {
        collection = null;
        return false;
    }
}
