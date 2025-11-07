using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using Robin.Contracts.Variables;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Internals;

internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider) : IVariableSegmentVisitor<Type>
{
    private readonly ConcurrentDictionary<Type, IMemberAccessor?> memberCache = new();
    private readonly ConcurrentDictionary<Type, IIndexAccessor?> indexCache = new();

    private bool TryGetMemberAccessor(Type dataType, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {
        accessor = memberCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IMemberAccessor<>).MakeGenericType(key);
            IMemberAccessor? memberAccessor = (IMemberAccessor?)serviceProvider.GetService(genType);
            if (memberAccessor is null && (key.IsAssignableTo(typeof(IDictionary)) || (key.IsGenericType && key.GetGenericTypeDefinition().IsAssignableTo(typeof(IDictionary<,>)))))
                memberAccessor = DictionaryMemberAccessor.Instance;
            return memberAccessor;
        });
        return accessor is not null;
    }

    private bool TryGetIndexAccessor(Type dataType, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        accessor = indexCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IIndexAccessor<>).MakeGenericType(key);
            IIndexAccessor? indexAccessor = (IIndexAccessor?)serviceProvider.GetService(genType);
            if (indexAccessor is null && (key.IsAssignableTo(typeof(IList)) || key.IsArray))
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
