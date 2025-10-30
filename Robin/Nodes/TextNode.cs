namespace Robin.Nodes;

public readonly struct TextNode(string text) : INode
{
    public string Text { get; } = text;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitTextNode(this, args);
    }
}

