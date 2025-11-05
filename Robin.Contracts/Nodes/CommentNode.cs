namespace Robin.Contracts.Nodes;

public readonly struct CommentNode(Extract message) : INode
{
    public Extract Range { get; } = message;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitComment(this, args);
    }
}

