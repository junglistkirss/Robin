namespace Robin.Contracts.Expressions;

public sealed class LiteralExpressionNode(string constant) : IExpressionNode
{
    public string Constant { get; } = constant;

    public bool Accept<TArgs>(IExpressionNodeVisitor<TArgs> visitor, TArgs args, out object? value)
    {
        return visitor.VisitLiteral(this, args, out value);
    }
}
