using Robin.Contracts.Expressions;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Robin.Contracts.Nodes;

public sealed class SectionNode(IExpressionNode expression, ImmutableArray<INode> children, bool inverted = false) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public ImmutableArray<INode> Children { get; } = children;
    public bool Inverted { get; } = inverted;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitSection(this, args);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args)
    {
        visitor.VisitSection(this, args);
    }
}

