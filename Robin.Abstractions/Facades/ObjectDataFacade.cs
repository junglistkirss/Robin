using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class EnumerableIterator(object? value) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
                action(item);
        }
    }
}

internal sealed class ObjectDataFacade : IDataFacade
{
    public readonly static ObjectDataFacade Instance = new();
    private ObjectDataFacade() { }
    public bool IsTrue(object? value) => value is not null;
    public bool IsCollection(object? value, [NotNullWhen(true)] out IIterator? collection)
    {
        return IteratorCache.GetIterator(value, out collection);
    }
}
