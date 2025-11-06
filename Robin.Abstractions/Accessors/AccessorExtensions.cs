using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Extensions;
public static class MemberAccessorRegistry
{
    private static readonly ConcurrentDictionary<Type, IMemberAccessor> _map
        = new();

    public static void Add<T>(IMemberAccessor accessor)
    {
        if (accessor == null) throw new ArgumentNullException(nameof(accessor));
        _map[typeof(T)] = accessor;
    }

    public static IMemberAccessor? Get(Type type)
        => _map.TryGetValue(type, out var accessor) ? accessor : null;

    public static IMemberAccessor? Get<T>() => Get(typeof(T));
}

public static class IndexAccessorRegistry
{
    private static readonly ConcurrentDictionary<Type, IIndexAccessor> _map
        = new();

    public static void Add<T>(IIndexAccessor accessor)
    {
        if (accessor == null) throw new ArgumentNullException(nameof(accessor));
        _map[typeof(T)] = accessor;
    }

    public static IIndexAccessor? Get(Type type)
        => _map.TryGetValue(type, out var accessor) ? accessor : null;

    public static IIndexAccessor? Get<T>() => Get(typeof(T));
}
public static class AccessorExtensions
{
    public delegate bool TryGetMemberValue(string member, [NotNull] out Delegate? value);
    public delegate bool TryGetIndexValue(int index, [NotNull] out Delegate? value);

    private sealed class DelegatedMemberAccessor(TryGetMemberValue tryGetMemberValue) : IMemberAccessor
    {
        public bool TryGetMember(string name, [NotNull] out Delegate? value)
        {
            ArgumentNullException.ThrowIfNull(tryGetMemberValue);
            return tryGetMemberValue(name, out value);
        }
    }
    public static IServiceCollection AddMemberAccessor<T>(this IServiceCollection services, TryGetMemberValue tryGet)
    {
        ArgumentNullException.ThrowIfNull(tryGet);
        IMemberAccessor instance = new DelegatedMemberAccessor(tryGet);
        MemberAccessorRegistry.Add<T>(instance);
        return services;
    }

    private sealed class DelegatedIndexAccessor(TryGetIndexValue tryGetIndexValue) : IIndexAccessor
    {
        public bool TryGetIndex(int index, [NotNull] out Delegate value)
        {
            ArgumentNullException.ThrowIfNull(tryGetIndexValue);
            return tryGetIndexValue(index, out value);
        }
    }
    public static IServiceCollection AddIndexAccessor<T>(this IServiceCollection services, TryGetIndexValue tryGet)
    {
        ArgumentNullException.ThrowIfNull(tryGet);
        IIndexAccessor instance = new DelegatedIndexAccessor(tryGet);
        IndexAccessorRegistry.Add<T>(instance);

        return services;
        // .AddKeyedSingleton<IIndexAccessor>(typeof(T), instance);
    }

}