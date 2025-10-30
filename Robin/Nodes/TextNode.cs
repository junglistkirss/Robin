namespace Robin.Nodes;

public readonly struct TextNode(string text) : INode
{
    public string Text { get; } = text;
}

