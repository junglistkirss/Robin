using Robin.Variables;

namespace Robin.Expressions;
public readonly struct IdentifierExpressionNode(AccesorPath path) : IExpressionNode
{
    public AccesorPath Path { get; } = path;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitIdenitifer(this, args);
    }
};
