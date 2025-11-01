using Robin.Contracts.Context;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin;

public sealed class ExpressionNodeVisitor(IAccessorVisitor<EvaluationResult, DataContext> accessorVisitor) : IExpressionNodeVisitor<EvaluationResult, DataContext>
{
    public EvaluationResult VisitBinaryOperation(BinaryOperationExpressionNode node, DataContext args)
    {
        object? left = node.Left.Accept(this, args);
        object? right = node.Right.Accept(this, args);

        switch (node.Operator)
        {
            case BinaryOperator.Add:
                break;
            case BinaryOperator.Subtract:
                break;
            case BinaryOperator.Multiply:
                break;
            case BinaryOperator.Divide:
                break;
            case BinaryOperator.Power:
                break;
            case BinaryOperator.Modulus:
                break;
            case BinaryOperator.And:
                break;
            case BinaryOperator.Or:
                break;
            case BinaryOperator.Equal:
                break;
            case BinaryOperator.NotEqual:
                break;
            case BinaryOperator.GreaterThan:
                break;
            case BinaryOperator.LessThan:
                break;
            case BinaryOperator.GreaterThanOrEqual:
                break;
            case BinaryOperator.LessThanOrEqual:
                break;
            default:
                break;
        }
        throw new NotImplementedException();
    }

    public EvaluationResult VisitFunctionCall(FunctionCallNode node, DataContext args)
    {
        throw new NotImplementedException();
    }

    public EvaluationResult VisitIdenitifer(IdentifierExpressionNode node, DataContext args)
    {
        EvaluationResult result = node.Path.Accept(accessorVisitor, args);
        if(result.Status == ResoltionState.NotFound && args.Previous is not null)
        {
            EvaluationResult prevResult = node.Path.Accept(accessorVisitor, args.Previous);
            result = prevResult;
        }
        return result;
    }

    public EvaluationResult VisitLiteral(LiteralExpressionNode node, DataContext _)
    {
        return new EvaluationResult(ResoltionState.Found, node.Constant);
    }

    public EvaluationResult VisitNumber(NumberExpressionNode node, DataContext _)
    {
        return new EvaluationResult(ResoltionState.Found, node.Constant);
    }

    public EvaluationResult VisitUnaryOperation(UnaryOperationExpressionNode node, DataContext args)
    {
        throw new NotImplementedException();
    }
}
