using RobinMustache.Abstractions.Expressions;
using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Nodes;

public sealed class VariableNode(IExpressionNode expression, bool unescaped) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public bool IsUnescaped { get; } = unescaped;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitVariable(this, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args)
    {
        visitor.VisitVariable(this, args);
    }
}

