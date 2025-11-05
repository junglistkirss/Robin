using Robin.Contracts.Expressions;

namespace Robin.Contracts.Nodes;

public readonly struct PartialCallNode(Extract name, IExpressionNode expression) : INode
{
    public Extract PartialName { get; } = name;
    public IExpressionNode Expression { get; } = expression;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitPartialCall(this, args);
    }
}