namespace Robin.Contracts.Variables;

public readonly struct MemberSegment(string memberName) : IVariableSegment
{
    public string MemberName => memberName;

    public TOut Accept<TOut, TArgs>(IVariableSegmentVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitMember(this, args);
    }
}
