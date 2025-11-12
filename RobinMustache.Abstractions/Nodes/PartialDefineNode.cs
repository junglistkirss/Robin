using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Nodes;

public readonly struct PartialDefineNode(string name, ImmutableArray<INode> children) : INode
{
    public string PartialName { get; } = name;
    public ImmutableArray<INode> Children { get; } = children;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitPartialDefine(this, args);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args)
    {
        visitor.VisitPartialDefine(this, args);
    }
}
