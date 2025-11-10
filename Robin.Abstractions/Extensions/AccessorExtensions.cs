using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using static Robin.Abstractions.Extensions.AccessorBuilder;

namespace Robin.Abstractions.Extensions;
public static class AccessorExtensions
{
    public delegate bool TryGetMemberDelegateAccessor<T>(string member, out Delegate value);
    public delegate bool TryGetIndexDelegateAccessor<T>(int index, out Delegate value);


    private sealed class DelegatedMemberAccessor<T>(TryGetMemberDelegateAccessor<T> tryGetMemberValue) : IMemberDelegateAccessor<T>
    {
        public bool TryGetMember(string name, out Delegate value)
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
        public bool TryGetIndex(int index, out Delegate value)
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

    public static IServiceCollection AddMemberObjectAccessor<T>(this IServiceCollection services, TryGetMemberObjectAccessor<T> tryGet)
    {
        return services.AddSingleton(CreateMemberObjectAccessor(tryGet));
    }

    public static IServiceCollection AddIndexObjectAccessor<T>(this IServiceCollection services, TryGetIndexObjectAccessor<T> tryGet)
    {
        return services.AddSingleton(CreateIndexObjectAccessor(tryGet));
    }
}
