using System.Collections.Immutable;
using System.Text;

namespace Robin.Nodes;

public readonly struct SectionNode(IExpressionNode expression, ImmutableArray<INode> children, bool inverted = false) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public ImmutableArray<INode> Children { get; } = children;
    public bool Inverted { get; } = inverted;
}

