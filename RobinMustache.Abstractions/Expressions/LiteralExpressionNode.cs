using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Expressions;

public sealed class LiteralExpressionNode(string constant) : IExpressionNode
{
    public string Constant { get; } = constant;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Accept<TArgs>(IExpressionNodeVisitor<TArgs> visitor, TArgs args, out object? value)
    {
        return visitor.VisitLiteral(this, args, out value);
    }
}
