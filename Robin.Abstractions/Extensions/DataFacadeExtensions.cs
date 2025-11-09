using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Facades;
using Robin.Abstractions.Iterators;

namespace Robin.Abstractions.Extensions;

public static class DataFacadeExtensions
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

    public static IServiceCollection AddDataFacade<T>(this IServiceCollection services, IDataFacade<T> facade)
    {
        if (facade is null)
            throw new ArgumentNullException(nameof(facade));
        return services.AddSingleton<IDataFacade<T>>(facade);
    }
    public static IServiceCollection AddDataFacadeFactory<T>(this IServiceCollection services, DataFacadeFactory factory)
    {
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));
        return services.AddSingleton<IDataFacade<T>>(new TypedDataFacade<T>(factory));
    }
}
