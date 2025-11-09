using Robin.Abstractions.Accessors;
using Robin.Contracts.Variables;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Internals;

internal sealed class ServiceDelegateAccesorVisitor(IServiceProvider serviceProvider) : BaseAccessorVisitor
{
    private readonly ConcurrentDictionary<Type, IMemberDelegateAccessor?> memberCache = new();
    private readonly ConcurrentDictionary<Type, IIndexDelegateAccessor?> indexCache = new();

    private bool TryGetMemberAccessorDelegate(Type dataType, [NotNullWhen(true)] out IMemberDelegateAccessor? accessor)
    {
        accessor = memberCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IMemberDelegateAccessor<>).MakeGenericType(key);
            IMemberDelegateAccessor? memberAccessor = (IMemberDelegateAccessor?)serviceProvider.GetService(genType);
            if (memberAccessor is null && (key.IsAssignableTo(typeof(IDictionary)) || (key.IsGenericType && key.GetGenericTypeDefinition().IsAssignableTo(typeof(IDictionary<,>)))))
                memberAccessor = DictionaryMemberAccessor.Instance;
            return memberAccessor;
        });
        return accessor is not null;
    }

    private bool TryGetIndexAccessorDelegate(Type dataType, [NotNullWhen(true)] out IIndexDelegateAccessor? accessor)
    {
        accessor = indexCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IIndexDelegateAccessor<>).MakeGenericType(key);
            IIndexDelegateAccessor? indexAccessor = (IIndexDelegateAccessor?)serviceProvider.GetService(genType);
            if (indexAccessor is null && (key.IsAssignableTo(typeof(IList)) || key.IsArray))
                indexAccessor = ListIndexAccessor.Instance;

            return indexAccessor;
        });



        return accessor is not null;
    }

    public override bool VisitIndex(IndexSegment segment, Type args, [NotNull] out ChainableGetter getter)
    {
        if (TryGetIndexAccessorDelegate(args, out IIndexDelegateAccessor? typedAccessor) && typedAccessor.TryGetIndex(segment.Index, out Delegate @delegate))
        {
            getter = @delegate.AsChainable();
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }

    public override bool VisitMember(MemberSegment segment, Type args, [NotNull] out ChainableGetter getter)
    {
        if (TryGetMemberAccessorDelegate(args, out IMemberDelegateAccessor? typedAccessor) && typedAccessor.TryGetMember(segment.MemberName, out Delegate @delegate))
        {
            getter = @delegate.AsChainable();
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }
}
