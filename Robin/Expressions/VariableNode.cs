using Robin.Nodes;

namespace Robin.Expressions;

public readonly struct VariableNode(IExpressionNode expression, bool unescaped) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public bool IsUnescaped { get; } = unescaped;
}

