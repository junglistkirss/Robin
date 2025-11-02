namespace Robin.Contracts.Variables;

public readonly struct KeyAccessor(VariablePath key) : IAccessor
{
    public VariablePath Key => key;
    public TOut Accept<TOut, TArgs>(IAccessorVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitKey(this, args);
    }
}
