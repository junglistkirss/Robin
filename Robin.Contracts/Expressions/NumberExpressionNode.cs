namespace Robin.Contracts.Expressions;

public readonly struct NumberExpressionNode(double constant) : IExpressionNode
{
    public double Constant { get; } = constant;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitNumber(this, args);
    }
}
