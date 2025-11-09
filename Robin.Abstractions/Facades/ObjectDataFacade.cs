using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class ObjectDataFacade : IDataFacade
{
    public readonly static ObjectDataFacade Instance = new();
    private ObjectDataFacade() { }
    public bool IsTrue([NotNullWhen(true)] object? value) => value is not null;
    public bool IsCollection(object? value, [NotNullWhen(true)] out IIterator? collection)
    {
        return IteratorCache.GetIterator(value, out collection);
    }
}
