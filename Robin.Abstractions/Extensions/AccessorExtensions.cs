using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using Robin.Abstractions.Facades;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using static Robin.Abstractions.Extensions.AccessorExtensions;

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
    public static IServiceCollection AddDataFacade<T>(this IServiceCollection services, DataFacadeFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return services.AddSingleton<IDataFacade<T>>(new TypedDataFacade<T>(factory));
    }
}
public static class AccessorExtensions
{
    public delegate bool TryGetMemberValue<T>(string member, [NotNull] out Delegate value);
    public delegate bool TryGetIndexValue<T>(int index, [NotNull] out Delegate value);

    private sealed class DelegatedMemberAccessor<T>(TryGetMemberValue<T> tryGetMemberValue) : IMemberAccessor<T>
    {
        public bool TryGetMember(string name, [NotNull] out Delegate value)
        {
            ArgumentNullException.ThrowIfNull(tryGetMemberValue);
            return tryGetMemberValue(name, out value);
        }
    }
    public static IServiceCollection AddMemberAccessor<T>(this IServiceCollection services, TryGetMemberValue<T> tryGet)
    {
        ArgumentNullException.ThrowIfNull(tryGet);
        return services.AddSingleton<IMemberAccessor<T>>(new DelegatedMemberAccessor<T>(tryGet));
    }

    private sealed class DelegatedIndexAccessor<T>(TryGetIndexValue<T> tryGetIndexValue) : IIndexAccessor<T>
    {
        public bool TryGetIndex(int index, [NotNull] out Delegate value)
        {
            ArgumentNullException.ThrowIfNull(tryGetIndexValue);
            return tryGetIndexValue(index, out value);
        }
    }
    public static IServiceCollection AddIndexAccessor<T>(this IServiceCollection services, TryGetIndexValue<T> tryGet)
    {
        ArgumentNullException.ThrowIfNull(tryGet);
        return services.AddSingleton<IIndexAccessor<T>>(new DelegatedIndexAccessor<T>(tryGet));
    }
}