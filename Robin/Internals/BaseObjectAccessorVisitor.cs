using Robin.Abstractions.Accessors;
using Robin.Abstractions.Variables;

namespace Robin.Internals;

internal abstract class BaseObjectAccessorVisitor : BaseAccessorVisitor
{
    protected abstract bool TryGetMemberAccessor(Type dataType, out IMemberAccessor? accessor);

    protected abstract bool TryGetIndexAccessor(Type dataType, out IIndexAccessor? accessor);

    public sealed override bool VisitIndex(IndexSegment segment, Type args, out ChainableGetter getter)
    {
        if (TryGetIndexAccessor(args, out IIndexAccessor? typedAccessor) && typedAccessor is not null)
        {
            int index = segment.Index;
            getter = new ChainableGetter((object? input, out object? value) =>
            {
                if (typedAccessor.TryGetIndex(input, index, out object? indexValue))
                {
                    value = indexValue;
                    return true;
                }
                value = null;
                return false;
            });
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }

    public sealed override bool VisitMember(MemberSegment segment, Type args, out ChainableGetter getter)
    {
        if (TryGetMemberAccessor(args, out IMemberAccessor? typedAccessor) && typedAccessor is not null)
        {
            string memberName = segment.MemberName;
            getter = new ChainableGetter((object? input, out object? value) =>
            {
                if (typedAccessor.TryGetMember(input, memberName, out object? memberValue))
                {
                    value = memberValue;
                    return true;
                }
                value = null;
                return false;
            });
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }
}


