using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using Robin.Contracts.Variables;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Internals;

internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider, IMemoryCache cache) : IVariableSegmentVisitor<Type>
{
    private bool TryGetMemberAccessor(Type dataType, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {

        accessor = cache.GetOrCreate(dataType, (entry) =>
        {
            IMemberAccessor? memberAccessor = serviceProvider.GetKeyedService<IMemberAccessor>(entry.Key);
            if (memberAccessor is null && entry.Key is Type type
                && (
                    type.IsAssignableTo(typeof(IDictionary))
                    || (
                        type.IsGenericType
                        && type.GetGenericTypeDefinition().IsAssignableTo(typeof(IDictionary<,>))
                    )
                )
            )
                memberAccessor = DictionaryMemberAccessor.Instance;
            return memberAccessor;
        });
        return accessor is not null;
    }

    private bool TryGetIndexAccessor(Type dataType, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        accessor = cache.GetOrCreate(dataType, (entry) =>
        {
            IIndexAccessor? indexAccessor = serviceProvider.GetKeyedService<IIndexAccessor>(entry.Key);
            if (indexAccessor is null && entry.Key is Type type && (type.IsAssignableTo(typeof(IList)) || type.IsArray))
                indexAccessor = ListIndexAccessor.Instance;

            return indexAccessor;
        });



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
