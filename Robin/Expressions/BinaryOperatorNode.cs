namespace Robin.Expressions;

public readonly struct BinaryOperationNode(IExpressionNode left, string @operator, IExpressionNode right) : IExpressionNode
{
    public string Operator { get; } = @operator;
    public IExpressionNode Left { get; } = left;
    public IExpressionNode Right { get; } = right;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitBinaryOperation(this, args);
    }
}

