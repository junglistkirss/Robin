namespace Robin.Contracts.Variables;

public sealed class ThisSegment : IVariableSegment
{
    public readonly static ThisSegment Instance = new();
    public bool Accept<TArgs>(IVariableSegmentVisitor<TArgs> visitor, TArgs args, out Delegate @delegate)
    {
        return visitor.VisitThis(this, args, out @delegate);
    }
}
