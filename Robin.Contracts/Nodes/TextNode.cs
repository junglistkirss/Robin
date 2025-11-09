using System.Runtime.CompilerServices;

namespace Robin.Contracts.Nodes;

public sealed class TextNode(string text) : INode
{
    public string Text { get; } = text;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitText(this, args);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args)
    {
        visitor.VisitText(this, args);
    }
}
