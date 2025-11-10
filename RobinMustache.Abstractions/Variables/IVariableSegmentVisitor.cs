namespace RobinMustache.Abstractions.Variables;

public interface IVariableSegmentVisitor<in TArgs, TOut>
{
    bool VisitThis(ThisSegment accessor, TArgs args, out TOut result);
    bool VisitMember(MemberSegment accessor, TArgs args, out TOut result);
    bool VisitIndex(IndexSegment accessor, TArgs args, out TOut result);
}