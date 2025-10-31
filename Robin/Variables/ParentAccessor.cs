namespace Robin.Variables;

public readonly struct ParentAccessor : IAccessor
{
    public TOut Accept<TOut, TArgs>(IAccessorVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitParent(this, args);
    }
}
