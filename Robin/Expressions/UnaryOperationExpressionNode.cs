namespace Robin.Expressions;

public readonly struct UnaryOperationExpressionNode(string @operator, IExpressionNode operand) : IExpressionNode
{
    public string Operator { get; } = @operator;
    public IExpressionNode Operand { get; } = operand;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitUnaryOperation(this, args);
    }
}