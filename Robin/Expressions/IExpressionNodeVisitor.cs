using Robin.Nodes;

namespace Robin.Expressions;

public interface IExpressionNodeVisitor<TOut, TArgs>
{
    TOut VisitIdenitifer(IdentifierExpressionNode node, TArgs args);
    TOut VisitFunctionCall(FunctionCallNode node, TArgs args);
    TOut VisitNumber(NumberExpressionNode node, TArgs args);
    TOut VisitLiteral(LiteralExpressionNode node, TArgs args);
    TOut VisitVariable(VariableNode node, TArgs args);
    TOut VisitBinaryOperation(BinaryOperationExpressionNode node, TArgs args);
    TOut VisitUnaryOperation(UnaryOperationExpressionNode node, TArgs args);
}