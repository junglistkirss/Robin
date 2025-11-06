namespace Robin.Contracts.Variables;

public sealed class IndexSegment(int index) : IVariableSegment
{
    public int Index => index;

    public bool Accept<TArgs>(IVariableSegmentVisitor<TArgs> visitor, TArgs args, out Delegate @delegate)
    {
        return visitor.VisitIndex(this, args, out @delegate);
    }
}
