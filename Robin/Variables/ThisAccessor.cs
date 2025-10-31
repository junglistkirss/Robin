namespace Robin.Variables;

public readonly struct ThisAccessor : IAccessor
{
    public TOut Accept<TOut, TArgs>(IAccessorVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitThis(this, args);
    }
}
