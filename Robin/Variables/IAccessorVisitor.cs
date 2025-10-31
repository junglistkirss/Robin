namespace Robin.Variables;

public interface IAccessorVisitor<TOut, TArgs>
{
    TOut VisitThis(ThisAccessor accessor, TArgs args);
    TOut VisitParent(ParentAccessor accessor, TArgs args);
    TOut VisitMember(MemberAccessor accessor, TArgs args);
    TOut VisitIndex(IndexAccessor accessor, TArgs args);
    TOut VisitKey(KeyAccessor accessor, TArgs args);
}