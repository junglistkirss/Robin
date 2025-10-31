namespace Robin.Variables;

public readonly struct MemberAccessor(string memberName) : IAccessor
{
    public string MemberName => memberName;

    public TOut Accept<TOut, TArgs>(IAccessorVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitMember(this, args);
    }
}
