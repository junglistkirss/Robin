namespace Robin.Variables;

public readonly struct KeyAccessor(AccesorPath key) : IAccessor
{
    public AccesorPath Key => key;
    public TOut Accept<TOut, TArgs>(IAccessorVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitKey(this, args);
    }
}
