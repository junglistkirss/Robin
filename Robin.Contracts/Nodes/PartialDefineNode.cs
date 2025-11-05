using System.Collections.Immutable;

namespace Robin.Contracts.Nodes;

public readonly struct PartialDefineNode(Extract name, ImmutableArray<INode> children) : INode
{
    public Extract PartialName { get; } = name;
    public ImmutableArray<INode> Children { get; } = children;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitPartialDefine(this, args);
    }
}
