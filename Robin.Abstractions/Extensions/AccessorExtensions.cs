using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Extensions;
public static class AccessorExtensions
{
    public delegate bool TryGetMemberDelegateAccessor<T>(string member, [NotNull] out Delegate value);
    public delegate bool TryGetIndexDelegateAccessor<T>(int index, [NotNull] out Delegate value);

    public delegate bool TryGetMemberObjectAccessor<T>(T obj, string member, out object? value);
    public delegate bool TryGetIndexObjectAccessor<T>(T obj, int index, out object? value);

    private sealed class DelegatedMemberAccessor<T>(TryGetMemberDelegateAccessor<T> tryGetMemberValue) : IMemberDelegateAccessor<T>
    {
        public bool TryGetMember(string name, [NotNull] out Delegate value)
        {
            ArgumentNullException.ThrowIfNull(tryGetMemberValue);
            return tryGetMemberValue(name, out value);
        }
    }
    public static IServiceCollection AddMemberDelegateAccessor<T>(this IServiceCollection services, TryGetMemberDelegateAccessor<T> tryGet)
    {
        ArgumentNullException.ThrowIfNull(tryGet);
        return services.AddSingleton<IMemberDelegateAccessor<T>>(new DelegatedMemberAccessor<T>(tryGet));
    }

    private sealed class DelegatedIndexAccessor<T>(TryGetIndexDelegateAccessor<T> tryGetIndexValue) : IIndexDelegateAccessor<T>
    {
        public bool TryGetIndex(int index, [NotNull] out Delegate value)
        {
            ArgumentNullException.ThrowIfNull(tryGetIndexValue);
            return tryGetIndexValue(index, out value);
        }
    }
    public static IServiceCollection AddIndexAccessorDelegate<T>(this IServiceCollection services, TryGetIndexDelegateAccessor<T> tryGet)
    {
        ArgumentNullException.ThrowIfNull(tryGet);
        return services.AddSingleton<IIndexDelegateAccessor<T>>(new DelegatedIndexAccessor<T>(tryGet));
    }




    private sealed class ObjectMemberAccessor<T>(TryGetMemberObjectAccessor<T> tryGetMemberValue) : IMemberAccessor<T>
    {
        public bool TryGetMember(T obj, string name, out object? value)
        {
            ArgumentNullException.ThrowIfNull(tryGetMemberValue);
            return tryGetMemberValue(obj, name, out value);
        }
    }
    public static IServiceCollection AddMemberObjectAccessor<T>(this IServiceCollection services, TryGetMemberObjectAccessor<T> tryGet)
    {
        ArgumentNullException.ThrowIfNull(tryGet);
        return services.AddSingleton<IMemberAccessor<T>>(new ObjectMemberAccessor<T>(tryGet));
    }

    private sealed class ObjectIndexAccessor<T>(TryGetIndexObjectAccessor<T> tryGetIndexValue) : IIndexAccessor<T>
    {
        public bool TryGetIndex(T obj, int index, out object? value)
        {
            ArgumentNullException.ThrowIfNull(tryGetIndexValue);
            return tryGetIndexValue(obj, index, out value);
        }
    }
    public static IServiceCollection AddIndexObjectAccessor<T>(this IServiceCollection services, TryGetIndexObjectAccessor<T> tryGet)
    {
        ArgumentNullException.ThrowIfNull(tryGet);
        return services.AddSingleton<IIndexAccessor<T>>(new ObjectIndexAccessor<T>(tryGet));
    }
}