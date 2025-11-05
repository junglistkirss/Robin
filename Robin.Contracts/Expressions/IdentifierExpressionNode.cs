using Robin.Contracts.Variables;

namespace Robin.Contracts.Expressions;

public sealed  class IdentifierExpressionNode(VariablePath path) : IExpressionNode
{
    public VariablePath Path { get; } = path;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitIdenitifer(this, args);
    }
};
