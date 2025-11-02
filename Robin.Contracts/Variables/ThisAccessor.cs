namespace Robin.Contracts.Variables;

public readonly struct ThisAccessor : IAccessor
{
    public readonly static ThisAccessor Instance = new();
    public TOut Accept<TOut, TArgs>(IAccessorVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitThis(this, args);
    }
}
