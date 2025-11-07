using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Variables;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Internals;

internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider, IMemoryCache cache) : IVariableSegmentVisitor<Type>
{
    private record MemberAccesorCacheKey(Type ObjectType);
    private record IndexAccessorCacheKey(Type ObjectType);

    private bool TryGetMemberAccessor(Type dataType, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {
        accessor = cache.GetOrCreate(new MemberAccesorCacheKey(dataType), (_) =>
        {
            Type genType = typeof(IMemberAccessor<>).MakeGenericType(dataType);
            IMemberAccessor? memberAccessor = (IMemberAccessor?)serviceProvider.GetService(genType);
            if (memberAccessor is null && (dataType.IsAssignableTo(typeof(IDictionary)) || (dataType.IsGenericType && dataType.GetGenericTypeDefinition().IsAssignableTo(typeof(IDictionary<,>)))))
                memberAccessor = DictionaryMemberAccessor.Instance;
            return memberAccessor;
        }, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) });
        return accessor is not null;
    }

    private bool TryGetIndexAccessor(Type dataType, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        accessor = cache.GetOrCreate(new IndexAccessorCacheKey(dataType), (_) =>
        {
            Type genType = typeof(IIndexAccessor<>).MakeGenericType(dataType);
            IIndexAccessor? indexAccessor = (IIndexAccessor?)serviceProvider.GetService(genType);
            if (indexAccessor is null && (dataType.IsAssignableTo(typeof(IList)) || dataType.IsArray))
                indexAccessor = ListIndexAccessor.Instance;

            return indexAccessor;
        }, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) });



        return accessor is not null;
    }

    public bool VisitIndex(IndexSegment segment, Type args, [NotNull] out Delegate @delegate)
    {
        if (TryGetIndexAccessor(args, out IIndexAccessor? typedAccessor))
        {
            typedAccessor.TryGetIndex(segment.Index, out @delegate);
            return true;
        }
        @delegate = (Func<object?, object?>)((_) => null);
        return false;
    }

    public bool VisitMember(MemberSegment segment, Type args, [NotNull] out Delegate @delegate)
    {
        if (TryGetMemberAccessor(args, out IMemberAccessor? typedAccessor))
        {
            typedAccessor.TryGetMember(segment.MemberName, out @delegate);
            return true;
        }
        @delegate = (Func<object?, object?>)((_) => null);
        return false;
    }

    public bool VisitThis(ThisSegment segment, Type _, [NotNull] out Delegate @delegate)
    {
        @delegate = (Func<object?, object?>)(x => x);
        return true;
    }
}
