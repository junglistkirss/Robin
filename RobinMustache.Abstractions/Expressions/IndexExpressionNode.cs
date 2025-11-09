using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Expressions;

public sealed class IndexExpressionNode(int constant) : IExpressionNode
{
    public int Constant { get; } = constant;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Accept<TArgs>(IExpressionNodeVisitor<TArgs> visitor, TArgs args, out object? value)
    {
        return visitor.VisitIndex(this, args, out value);
    }
}
