namespace Robin.Contracts.Expressions;

public interface IExpressionNode
{
    TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args);
};
