using RobinMustache.Abstractions.Variables;
using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Expressions;

public sealed class IdentifierExpressionNode(VariablePath path) : IExpressionNode
{
    public VariablePath Path { get; } = path;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Accept<TArgs>(IExpressionNodeVisitor<TArgs> visitor, TArgs args, out object? value)
    {
        return visitor.VisitIdenitifer(this, args, out value);
    }
};
