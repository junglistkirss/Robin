namespace Robin.Contracts.Expressions;

public readonly struct LiteralExpressionNode(Extract constant) : IExpressionNode
{
    public Extract Constant { get; } = constant;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitLiteral(this, args);
    }
}
