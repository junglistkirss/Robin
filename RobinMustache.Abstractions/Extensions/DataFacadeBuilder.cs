using RobinMustache.Abstractions.Facades;
using RobinMustache.Abstractions.Iterators;

namespace RobinMustache.Abstractions.Extensions;

public static class DataFacadeBuilder
{
    public delegate IDataFacade DataFacadeFactory(object? obj);

    private sealed class TypedDataFacade<T>(DataFacadeFactory facadeFactory) : BaseDataFacade<T>
    {
        public override bool IsCollection(T obj, out IIterator? collection)
        {
            return facadeFactory(obj).IsCollection(obj, out collection);
        }

        public override bool IsTrue(T obj)
        {
            return facadeFactory(obj).IsTrue(obj);
        }
    }

    public static IDataFacade<T> CreateDataFacade<T>(DataFacadeFactory factory)
    {
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));
        return new TypedDataFacade<T>(factory);
    }
}