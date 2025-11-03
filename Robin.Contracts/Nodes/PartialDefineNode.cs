using System.Collections.Immutable;

namespace Robin.Contracts.Nodes;

public readonly struct PartialDefineNode(string name, ImmutableArray<INode> children) : INode
{
    public string PartialName { get; } = name;
    public ImmutableArray<INode> Children { get; } = children;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitPartialDefine(this, args);
    }
}
