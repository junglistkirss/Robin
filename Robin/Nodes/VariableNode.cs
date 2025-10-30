using Robin.Nodes.Expressions;

namespace Robin.Nodes;

public readonly struct VariableNode(IExpressionNode expression, bool unescaped) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public bool IsUnescaped { get; } = unescaped;

    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitVariableNode(this, args);
    }
}

