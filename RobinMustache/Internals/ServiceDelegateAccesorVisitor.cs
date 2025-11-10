using RobinMustache.Abstractions.Accessors;
using RobinMustache.Abstractions.Variables;
using System.Collections;
using System.Collections.Concurrent;

namespace RobinMustache.Internals;

internal sealed class ServiceDelegateAccesorVisitor(IServiceProvider serviceProvider) : BaseAccessorVisitor
{
    private readonly ConcurrentDictionary<Type, IMemberDelegateAccessor?> memberCache = new();
    private readonly ConcurrentDictionary<Type, IIndexDelegateAccessor?> indexCache = new();

    private bool TryGetMemberAccessorDelegate(Type dataType, out IMemberDelegateAccessor? accessor)
    {
        accessor = memberCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IMemberDelegateAccessor<>).MakeGenericType(key);
            IMemberDelegateAccessor? memberAccessor = (IMemberDelegateAccessor?)serviceProvider.GetService(genType);
            if (key.GetInterfaces().Any(x => x == typeof(IDictionary)))
                memberAccessor = DictionaryMemberAccessor.Instance;
            return memberAccessor;
        });
        return accessor is not null;
    }

    private bool TryGetIndexAccessorDelegate(Type dataType, out IIndexDelegateAccessor? accessor)
    {
        accessor = indexCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IIndexDelegateAccessor<>).MakeGenericType(key);
            IIndexDelegateAccessor? indexAccessor = (IIndexDelegateAccessor?)serviceProvider.GetService(genType);
            if (key.GetInterfaces().Any(x => x == typeof(IList)))
                indexAccessor = ListIndexAccessor.Instance;

            return indexAccessor;
        });



        return accessor is not null;
    }

    public override bool VisitIndex(IndexSegment segment, Type args, out ChainableGetter getter)
    {
        if (TryGetIndexAccessorDelegate(args, out IIndexDelegateAccessor? typedAccessor) && typedAccessor is not null && typedAccessor.TryGetIndex(segment.Index, out Delegate @delegate))
        {
            getter = @delegate.AsChainable();
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }

    public override bool VisitMember(MemberSegment segment, Type args, out ChainableGetter getter)
    {
        if (TryGetMemberAccessorDelegate(args, out IMemberDelegateAccessor? typedAccessor) && typedAccessor is not null && typedAccessor.TryGetMember(segment.MemberName, out Delegate @delegate))
        {
            getter = @delegate.AsChainable();
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }
}
