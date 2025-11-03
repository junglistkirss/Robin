namespace Robin.Contracts.Variables;

public interface IAccessorVisitor<TOut, TArgs>
{
    TOut VisitThis(ThisAccessor accessor, TArgs args);
    TOut VisitMember(MemberAccessor accessor, TArgs args);
    TOut VisitIndex(IndexAccessor accessor, TArgs args);
}