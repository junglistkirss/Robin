namespace Robin.Contracts.Variables;

public readonly struct MemberSegment(Extract memberName) : IVariableSegment
{
    public Extract MemberName => memberName;

    public TOut Accept<TOut, TArgs>(IVariableSegmentVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitMember(this, args);
    }
}
