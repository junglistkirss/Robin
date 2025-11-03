namespace Robin.Contracts.Variables;

public interface IVariableSegmentVisitor<TOut, TArgs>
{
    TOut VisitThis(ThisSegment accessor, TArgs args);
    TOut VisitMember(MemberISegment accessor, TArgs args);
    TOut VisitIndex(IndexSegment accessor, TArgs args);
}