namespace Robin.Contracts.Expressions;

public interface IExpressionNodeVisitor<TOut, TArgs>
{
    TOut VisitIdenitifer(IdentifierExpressionNode node, TArgs args);
    TOut VisitFunctionCall(FunctionCallNode node, TArgs args);
    TOut VisitIndex(NumberExpressionNode node, TArgs args);
    TOut VisitLiteral(LiteralExpressionNode node, TArgs args);
}