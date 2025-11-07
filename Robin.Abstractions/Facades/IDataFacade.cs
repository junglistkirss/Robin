using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

public interface IDataFacade
{
    bool IsTrue(object? obj);
    bool IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection);
}

public interface IDataFacade<T> : IDataFacade
{
    bool IDataFacade.IsTrue(object? obj) => obj is T typed && IsTrue(typed);
    bool IsTrue(T obj);
    bool IDataFacade.IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection)
    {
        if (obj is T typed && IsCollection(typed, out IIterator? typedCollection))
        {
            collection = typedCollection;
            return true;
        }
        collection = null;
        return false;
    }
    bool IsCollection(T obj, [NotNullWhen(true)] out IIterator? collection);
}