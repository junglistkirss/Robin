using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Variables;

public sealed class MemberSegment(string memberName) : IVariableSegment
{
    public string MemberName => memberName;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Accept<TArgs, TOut>(IVariableSegmentVisitor<TArgs, TOut> visitor, TArgs args, out TOut @delegate)
    {
        return visitor.VisitMember(this, args, out @delegate);
    }
}
