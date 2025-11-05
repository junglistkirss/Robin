using Robin.Contracts.Expressions;
using System.Collections.Immutable;

namespace Robin.Contracts.Nodes;

public sealed  class SectionNode(IExpressionNode expression, ImmutableArray<INode> children, bool inverted = false) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public ImmutableArray<INode> Children { get; } = children;
    public bool Inverted { get; } = inverted;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitSection(this, args);
    }
}

