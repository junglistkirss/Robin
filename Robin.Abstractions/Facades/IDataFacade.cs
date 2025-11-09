using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

public interface IDataFacade
{
    bool IsTrue(object? obj);
    bool IsCollection(object? obj, out IIterator? collection);
}

public interface IDataFacade<T> : IDataFacade
{
    bool IsTrue(T obj);
    bool IsCollection(T obj, out IIterator? collection);
}
