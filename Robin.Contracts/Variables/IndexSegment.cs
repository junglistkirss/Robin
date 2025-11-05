namespace Robin.Contracts.Variables;

public readonly struct IndexSegment(Extract index) : IVariableSegment
{
    public Extract Index => index;

    public TOut Accept<TOut, TArgs>(IVariableSegmentVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitIndex(this, args);
    }
}
