using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Extensions;

public static class AccessorExtensions
{

    public delegate bool TryGetMemberValue(string member, [NotNull] out Delegate? value);
    public delegate bool TryGetIndexValue(int index, [NotNull] out Delegate? value);

    private sealed class DelegatedMemberAccessor(TryGetMemberValue tryGetMemberValue) : IMemberAccessor
    {
        public bool TryGetMember(string name, [NotNull] out Delegate? value)
        {
            return tryGetMemberValue(name, out value);
        }
    }
    public static IServiceCollection AddMemberAccessor<T>(this IServiceCollection services, TryGetMemberValue tryGet)
    {
        return services
            .AddKeyedSingleton<IMemberAccessor>(typeof(T), new DelegatedMemberAccessor(tryGet));
    }

    private sealed class DelegatedIndexAccessor(TryGetIndexValue tryGetIndexValue) : IIndexAccessor
    {
        public bool TryGetIndex(int index, [NotNull] out Delegate value)
        {
            return tryGetIndexValue(index, out value);
        }
    }
    public static IServiceCollection AddIndexAccessor<T>(this IServiceCollection services, TryGetIndexValue tryGet)
    {
        return services
            .AddKeyedSingleton<IIndexAccessor>(typeof(T), new DelegatedIndexAccessor(tryGet));
    }

}