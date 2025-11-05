namespace Robin.Contracts.Nodes;

public sealed  class LineBreakNode(int count) : INode
{
    public int Count { get; } = count;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitLineBreak(this, args);
    }
}
