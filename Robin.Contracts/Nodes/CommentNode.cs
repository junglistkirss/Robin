namespace Robin.Contracts.Nodes;

public sealed  class CommentNode(string message) : INode
{
    public string Message { get; } = message;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitComment(this, args);
    }
}

