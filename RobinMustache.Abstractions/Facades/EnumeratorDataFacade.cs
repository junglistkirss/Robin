using RobinMustache.Abstractions.Iterators;

namespace RobinMustache.Abstractions.Facades;

internal sealed class EnumeratorDataFacade : IDataFacade
{
    public readonly static EnumeratorDataFacade Instance = new();
    private EnumeratorDataFacade() { }
    public bool IsTrue(object? _) => true;
    public bool IsCollection(object? obj, out IIterator? collection)
    {
        collection = IEnumeratorIterator.Instance;
        return true;
    }
}
