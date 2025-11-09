using System.Runtime.CompilerServices;

namespace Robin.Abstractions.Variables;

public sealed class ThisSegment : IVariableSegment
{
    public readonly static ThisSegment Instance = new();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Accept<TArgs, TOut>(IVariableSegmentVisitor<TArgs, TOut> visitor, TArgs args, out TOut @delegate)
    {
        return visitor.VisitThis(this, args, out @delegate);
    }
}
