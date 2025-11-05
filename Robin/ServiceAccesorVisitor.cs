using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using Robin.Contracts.Variables;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;

internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider, IMemoryCache cache) : IVariableSegmentVisitor<Type>
{
    private bool TryGetMemberAccessor(Type dataType, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {
        if (dataType == typeof(IDictionary))
        {
            accessor = DictionaryMemberAccessor.Instance;
        }
        else
        {
            accessor = cache.GetOrCreate(dataType, (t) =>
            {
                IMemberAccessor? memberAccessor = serviceProvider.GetKeyedService<IMemberAccessor>(t.Key);
                if (memberAccessor is not null)
                    return memberAccessor;


                return memberAccessor;
            });
        }

        return accessor is not null;
    }

    private bool TryGetIndexAccessor(Type dataType, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        if (dataType == typeof(IList))
        {
            accessor = ListIndexAccessor.Instance;
            return true;
        }
        else
        {
            accessor = cache.GetOrCreate(dataType, (t) =>
            {
                IIndexAccessor? indexAccessor = serviceProvider.GetKeyedService<IIndexAccessor>(t.Key);
                if (indexAccessor is not null)
                    return indexAccessor;


                return indexAccessor;
            });
        }


        return accessor is not null;
    }

    public bool VisitIndex(IndexSegment segment, Type args, [NotNull] out Delegate @delegate)
    {
        if (TryGetIndexAccessor(args, out IIndexAccessor? typedAccessor))
        {
            typedAccessor.TryGetIndex(segment.Index, out @delegate);
            return true;
        }
        @delegate = (Func<object?, object?>)((object? _) => null);
        return false;
    }

    public bool VisitMember(MemberSegment segment, Type args, [NotNull] out Delegate @delegate)
    {
        if (TryGetMemberAccessor(args, out IMemberAccessor? typedAccessor))
        {
            typedAccessor.TryGetMember(segment.MemberName, out @delegate);
            return true;
        }
        @delegate = (Func<object?, object?>)((object? _) => null);
        return false;
    }

    public bool VisitThis(ThisSegment segment, Type _, [NotNull] out Delegate @delegate)
    {
        @delegate = (Func<object?, object?>)(x => x);
        return true;
    }
}
