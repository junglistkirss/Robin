namespace Robin.Abstractions.Expressions;

public interface IExpressionNode
{
    bool Accept<TArgs>(IExpressionNodeVisitor<TArgs> visitor, TArgs args, out object? value);
};
