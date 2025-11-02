using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

public sealed class ExpressionNodeVisitor(IAccessorVisitor<EvaluationResult, DataContext> accessorVisitor) : IExpressionNodeVisitor<EvaluationResult, DataContext>
{
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
}
