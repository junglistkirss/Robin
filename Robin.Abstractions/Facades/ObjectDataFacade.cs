using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class EnumerableIterator(IEnumerable objects) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        foreach (var item in objects)
            action(item);
    }
}

internal sealed class ObjectDataFacade : IDataFacade
{
    public readonly static ObjectDataFacade Instance = new();
    private ObjectDataFacade() { }
    public bool IsTrue(object? value) => value is not null;
    public bool IsCollection(object? value, [NotNullWhen(true)] out IIterator? collection)
    {
        if (value is IEnumerable enumerable)
        {
            collection = new EnumerableIterator(enumerable);
            return true;
        }
        collection = null;
        return false;
    }
}
