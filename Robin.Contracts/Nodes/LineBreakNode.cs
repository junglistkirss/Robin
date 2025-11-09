using System.Runtime.CompilerServices;

namespace Robin.Contracts.Nodes;

public sealed class LineBreakNode(int count) : INode
{
    public int Count { get; } = count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitLineBreak(this, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args)
    {
        visitor.VisitLineBreak(this, args);
    }
}
