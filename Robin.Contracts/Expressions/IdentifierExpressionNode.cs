using Robin.Contracts.Variables;

namespace Robin.Contracts.Expressions;

public sealed class IdentifierExpressionNode(VariablePath path) : IExpressionNode
{
    public VariablePath Path { get; } = path;

    public bool Accept<TArgs>(IExpressionNodeVisitor<TArgs> visitor, TArgs args, out object? value)
    {
        return visitor.VisitIdenitifer(this, args, out value);
    }
};
