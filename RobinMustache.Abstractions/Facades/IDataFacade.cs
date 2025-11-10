using RobinMustache.Abstractions.Iterators;

namespace RobinMustache.Abstractions.Facades;

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
