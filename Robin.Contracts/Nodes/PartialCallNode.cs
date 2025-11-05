using Robin.Contracts.Expressions;

namespace Robin.Contracts.Nodes;

public sealed  class PartialCallNode(string name, IExpressionNode expression) : INode
{
    public string PartialName { get; } = name;
    public IExpressionNode Expression { get; } = expression;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitPartialCall(this, args);
    }
}