using Robin.Abstractions.Variables;

namespace Robin.Abstractions.Accessors;

public abstract class BaseAccessorVisitor : IVariableSegmentVisitor<Type, ChainableGetter>
{
    public abstract bool VisitIndex(IndexSegment accessor, Type args, out ChainableGetter result);
    public abstract bool VisitMember(MemberSegment accessor, Type args, out ChainableGetter result);

    public bool VisitThis(ThisSegment segment, Type args, out ChainableGetter getter)
    {
        getter = new ChainableGetter((object? input, out object? value) =>
        {
            value = input;
            return true;
        });
        return true;
    }
}