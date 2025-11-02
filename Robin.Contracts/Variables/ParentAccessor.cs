namespace Robin.Contracts.Variables;

public readonly struct ParentAccessor : IAccessor
{
    public readonly static ParentAccessor Instance = new();
    public TOut Accept<TOut, TArgs>(IAccessorVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitParent(this, args);
    }
}
