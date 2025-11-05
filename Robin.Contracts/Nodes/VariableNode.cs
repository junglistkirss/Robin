using Robin.Contracts.Expressions;

namespace Robin.Contracts.Nodes;

public sealed  class VariableNode(IExpressionNode expression, bool unescaped) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public bool IsUnescaped { get; } = unescaped;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitVariable(this, args);
    }
}

