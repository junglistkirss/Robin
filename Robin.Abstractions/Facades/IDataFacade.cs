using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

public interface IDataFacade
{
    bool IsTrue([NotNullWhen(true)] object? obj);
    bool IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection);
}

public interface IDataFacade<T> : IDataFacade
{
    bool IDataFacade.IsTrue([NotNullWhen(true)] object? obj) => obj is T typed && IsTrue(typed);
    bool IsTrue([NotNullWhen(true)] T obj);
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