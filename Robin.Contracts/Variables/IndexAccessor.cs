namespace Robin.Contracts.Variables;

public readonly struct IndexAccessor(int index) : IAccessor
{
    public int Index => index;

    public TOut Accept<TOut, TArgs>(IAccessorVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitIndex(this, args);
    }
}
