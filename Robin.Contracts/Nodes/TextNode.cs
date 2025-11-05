namespace Robin.Contracts.Nodes;

public readonly struct TextNode(Extract text) : INode
{
    public Extract Text { get; } = text;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitText(this, args);
    }
}
