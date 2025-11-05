namespace Robin.Contracts.Variables;

public  sealed class ThisSegment : IVariableSegment
{
    public readonly static ThisSegment Instance = new();
    public TOut Accept<TOut, TArgs>(IVariableSegmentVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitThis(this, args);
    }
}
