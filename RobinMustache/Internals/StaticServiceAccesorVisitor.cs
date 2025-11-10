using RobinMustache.Abstractions.Accessors;
using System.Collections.Concurrent;

namespace RobinMustache.Internals;

internal sealed class StaticServiceAccesorVisitor(
    IEnumerable<KeyValuePair<Type, IMemberAccessor>> memberAccessors,
    IEnumerable<KeyValuePair<Type, IIndexAccessor>> indexAccessors) : BaseObjectAccessorVisitor
{
    private readonly ConcurrentDictionary<Type, IMemberAccessor> memberCache = new(memberAccessors);
    private readonly ConcurrentDictionary<Type, IIndexAccessor> indexCache = new(indexAccessors);

    protected override bool TryGetMemberAccessor(Type dataType, out IMemberAccessor? accessor)
    {
        return memberCache.TryGetValue(dataType, out accessor);
    }

    protected override bool TryGetIndexAccessor(Type dataType, out IIndexAccessor? accessor)
    {
        return indexCache.TryGetValue(dataType, out accessor);
    }
}


