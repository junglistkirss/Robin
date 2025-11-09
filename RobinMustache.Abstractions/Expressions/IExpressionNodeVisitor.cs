namespace RobinMustache.Abstractions.Expressions;

public interface IExpressionNodeVisitor<TArgs>
{
    bool VisitIdenitifer(IdentifierExpressionNode node, TArgs args, out object? value);
    bool VisitFunctionCall(FunctionCallNode node, TArgs args, out object? value);
    bool VisitIndex(IndexExpressionNode node, TArgs args, out object? value);
    bool VisitLiteral(LiteralExpressionNode node, TArgs args, out object? value);
}