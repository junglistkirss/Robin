namespace Robin.Contracts.Variables;

public readonly struct IndexSegment(int index) : IVariableSegment
{
    public int Index => index;

    public TOut Accept<TOut, TArgs>(IVariableSegmentVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitIndex(this, args);
    }
}
