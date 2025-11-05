namespace Robin.Contracts.Expressions;

public sealed  class NumberExpressionNode(int constant) : IExpressionNode
{
    public int Constant { get; } = constant;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitIndex(this, args);
    }
}
