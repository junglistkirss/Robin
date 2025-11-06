namespace Robin.Contracts.Expressions;

public sealed class IndexExpressionNode(int constant) : IExpressionNode
{
    public int Constant { get; } = constant;

    public bool Accept<TArgs>(IExpressionNodeVisitor<TArgs> visitor, TArgs args, out object? value)
    {
        return visitor.VisitIndex(this, args, out value);
    }
}
