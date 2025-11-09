using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions.Accessors;

namespace RobinMustache.Abstractions.Extensions;
public static class AccessorExtensions
{
    public delegate bool TryGetMemberDelegateAccessor<T>(string member,  out Delegate value);
    public delegate bool TryGetIndexDelegateAccessor<T>(int index,  out Delegate value);

    public delegate bool TryGetMemberObjectAccessor<T>(T obj, string member, out object? value);
    public delegate bool TryGetIndexObjectAccessor<T>(T obj, int index, out object? value);

    private sealed class DelegatedMemberAccessor<T>(TryGetMemberDelegateAccessor<T> tryGetMemberValue) : IMemberDelegateAccessor<T>
    {
        public bool TryGetMember(string name,  out Delegate value)
        {
            if (tryGetMemberValue is null)
                throw new ArgumentNullException(nameof(tryGetMemberValue));
            return tryGetMemberValue(name, out value);
        }
    }
    public static IServiceCollection AddMemberDelegateAccessor<T>(this IServiceCollection services, TryGetMemberDelegateAccessor<T> tryGet)
    {
        if (tryGet is null)
            throw new ArgumentNullException(nameof(tryGet));
        return services.AddSingleton<IMemberDelegateAccessor<T>>(new DelegatedMemberAccessor<T>(tryGet));
    }

    private sealed class DelegatedIndexAccessor<T>(TryGetIndexDelegateAccessor<T> tryGetIndexValue) : IIndexDelegateAccessor<T>
    {
        public bool TryGetIndex(int index,  out Delegate value)
        {
            if (tryGetIndexValue is null)
                throw new ArgumentNullException(nameof(tryGetIndexValue));
            return tryGetIndexValue(index, out value);
        }
    }
    public static IServiceCollection AddIndexAccessorDelegate<T>(this IServiceCollection services, TryGetIndexDelegateAccessor<T> tryGet)
    {
        if (tryGet is null)
            throw new ArgumentNullException(nameof(tryGet));
        return services.AddSingleton<IIndexDelegateAccessor<T>>(new DelegatedIndexAccessor<T>(tryGet));
    }




    private sealed class ObjectMemberAccessor<T>(TryGetMemberObjectAccessor<T> tryGetMemberValue) : BaseMemberAccessor<T>
    {
        public override bool TryGetMember(T obj, string name, out object? value)
        {
            if (tryGetMemberValue is null)
                throw new ArgumentNullException(nameof(tryGetMemberValue));
            return tryGetMemberValue(obj, name, out value);
        }
    }
    public static IServiceCollection AddMemberObjectAccessor<T>(this IServiceCollection services, TryGetMemberObjectAccessor<T> tryGet)
    {
        if (tryGet is null)
            throw new ArgumentNullException(nameof(tryGet));
        return services.AddSingleton<IMemberAccessor<T>>(new ObjectMemberAccessor<T>(tryGet));
    }

    private sealed class ObjectIndexAccessor<T>(TryGetIndexObjectAccessor<T> tryGetIndexValue) : BaseIndexAccessor<T>
    {
        public override bool TryGetIndex(T obj, int index, out object? value)
        {
            if (tryGetIndexValue is null)
                throw new ArgumentNullException(nameof(tryGetIndexValue));
            return tryGetIndexValue(obj, index, out value);
        }
    }
    public static IServiceCollection AddIndexObjectAccessor<T>(this IServiceCollection services, TryGetIndexObjectAccessor<T> tryGet)
    {
        if (tryGet is null)
            throw new ArgumentNullException(nameof(tryGet));
        return services.AddSingleton<IIndexAccessor<T>>(new ObjectIndexAccessor<T>(tryGet));
    }
}