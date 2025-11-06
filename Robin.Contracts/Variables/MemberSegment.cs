namespace Robin.Contracts.Variables;

public sealed class MemberSegment(string memberName) : IVariableSegment
{
    public string MemberName => memberName;

    public bool Accept<TArgs>(IVariableSegmentVisitor<TArgs> visitor, TArgs args, out Delegate @delegate)
    {
        return visitor.VisitMember(this, args, out @delegate);
    }
}
