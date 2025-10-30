using Robin.Nodes.Expressions;
using System.Collections.Immutable;

namespace Robin.Nodes;

public readonly struct SectionNode(IExpressionNode expression, ImmutableArray<INode> children, bool inverted = false) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public ImmutableArray<INode> Children { get; } = children;
    public bool Inverted { get; } = inverted;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitSectionNode(this, args);
    }
}

