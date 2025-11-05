using System.Diagnostics.CodeAnalysis;

namespace Robin.Contracts.Variables;

public interface IVariableSegmentVisitor<in TArgs>
{
    bool VisitThis(ThisSegment accessor, TArgs args, [NotNull] out Delegate @delegate);
    bool VisitMember(MemberSegment accessor, TArgs args, [NotNull] out Delegate @delegate);
    bool VisitIndex(IndexSegment accessor, TArgs args, [NotNull] out Delegate @delegate);
}