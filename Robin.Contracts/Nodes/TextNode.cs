namespace Robin.Contracts.Nodes;

public sealed  class TextNode(string text) : INode
{
    public string Text { get; } = text;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitText(this, args);
    }
}
