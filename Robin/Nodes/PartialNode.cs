using System.Collections.Immutable;

namespace Robin.Nodes;

public readonly struct PartialNode(string name, ImmutableArray<INode> children) : INode
{
    public string Name { get; } = name;
    public ImmutableArray<INode> Children { get; } = children;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitPartial(this, args);
    }
}

