using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Variables;

public sealed class IndexSegment(int index) : IVariableSegment
{
    public int Index => index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Accept<TArgs, TOut>(IVariableSegmentVisitor<TArgs, TOut> visitor, TArgs args, out TOut @delegate)
    {
        return visitor.VisitIndex(this, args, out @delegate);
    }
}
