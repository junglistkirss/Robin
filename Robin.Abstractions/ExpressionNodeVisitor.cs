using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

public sealed class ExpressionNodeVisitor(IAccessorVisitor<EvaluationResult, DataContext> accessorVisitor) : IExpressionNodeVisitor<EvaluationResult, DataContext>
{
    public EvaluationResult VisitBinaryOperation(BinaryOperationExpressionNode node, DataContext args)
    {
        EvaluationResult left = node.Left.Accept(this, args);
        EvaluationResult right = node.Right.Accept(this, args);

        switch (node.Operator)
        {
            //case BinaryOperator.Add:
            //    break;
            //case BinaryOperator.Subtract:
            //    break;
            //case BinaryOperator.Multiply:
            //    break;
            //case BinaryOperator.Divide:
            //    break;
            //case BinaryOperator.Power:
            //    break;
            //case BinaryOperator.Modulus:
            //    break;
            case BinaryOperator.And:
                if (left.Status == ResoltionState.Found && right.Status == ResoltionState.Found)
                    return new EvaluationResult(ResoltionState.Found, new BooleanDataFacade(left.Value.IsTrue() && right.Value.IsTrue()));
                return new EvaluationResult(ResoltionState.Found, BooleanDataFacade.False);
            case BinaryOperator.Or:
                if (left.Status != ResoltionState.Found && right.Status != ResoltionState.Found)
                    return new EvaluationResult(ResoltionState.NotFound, DataFacade.Null);
                if (left.Status == ResoltionState.Found || right.Status == ResoltionState.Found)
                    return new EvaluationResult(ResoltionState.Found, new BooleanDataFacade(left.Value.IsTrue() || right.Value.IsTrue()));
                return new EvaluationResult(ResoltionState.Found, BooleanDataFacade.False);
            //case BinaryOperator.Equal:
            //    break;
            //case BinaryOperator.NotEqual:
            //    break;
            //case BinaryOperator.GreaterThan:
            //    break;
            //case BinaryOperator.LessThan:
            //    break;
            //case BinaryOperator.GreaterThanOrEqual:
            //    break;
            //case BinaryOperator.LessThanOrEqual:
            //    break;
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
        EvaluationResult result = node.Path.Evaluate(accessorVisitor, args);
        if (result.Status == ResoltionState.NotFound && args.Parent is not null)
        {
            EvaluationResult prevResult = node.Path.Evaluate(accessorVisitor, args.Parent);
            result = prevResult;
        }
        return result;
    }

    public EvaluationResult VisitLiteral(LiteralExpressionNode node, DataContext _)
    {
        return new EvaluationResult(ResoltionState.Found, node.Constant.AsFacade());
    }

    public EvaluationResult VisitNumber(NumberExpressionNode node, DataContext _)
    {
        return new EvaluationResult(ResoltionState.Found, node.Constant.AsFacade());
    }

    public EvaluationResult VisitUnaryOperation(UnaryOperationExpressionNode node, DataContext args)
    {
        EvaluationResult operand = node.Operand.Accept(this, args);
        throw new NotImplementedException();
    }
}
