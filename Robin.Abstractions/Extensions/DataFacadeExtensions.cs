using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Facades;
using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Extensions;

public static class DataFacadeExtensions
{
    public delegate IDataFacade DataFacadeFactory(object? obj);

    private sealed class TypedDataFacade<T>(DataFacadeFactory facadeFactory) : IDataFacade<T>
    {
        public bool IsCollection(T obj, [NotNullWhen(true)] out IIterator? collection)
        {
            return facadeFactory(obj).IsCollection(obj, out collection);
        }

        public bool IsTrue(T obj)
        {
            return facadeFactory(obj).IsTrue(obj);
        }

    }

    public static IServiceCollection AddDataFacade<T>(this IServiceCollection services, IDataFacade<T> facade)
    {
        ArgumentNullException.ThrowIfNull(facade);
        return services.AddSingleton<IDataFacade<T>>(facade);
    }
    public static IServiceCollection AddDataFacadeFactory<T>(this IServiceCollection services, DataFacadeFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return services.AddSingleton<IDataFacade<T>>(new TypedDataFacade<T>(factory));
    }
}
