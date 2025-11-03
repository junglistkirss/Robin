using Robin.Abstractions.Facades;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

public sealed class ExpressionNodeVisitor(IVariableSegmentVisitor<EvaluationResult, object?> accessorVisitor) : IExpressionNodeVisitor<EvaluationResult, DataContext>
{
    public EvaluationResult VisitFunctionCall(FunctionCallNode node, DataContext args)
    {
        if(Helper.TryGetFunction(node.FunctionName, out Helper.Function? function) && function is not null)
        {
            object?[] evaluatedArgs = new object?[node.Arguments.Length];
            for (int i = 0; i < node.Arguments.Length; i++)
            {
                EvaluationResult evalResult = node.Arguments[i].Accept(this, args);
                if (evalResult.Status == ResoltionState.Found)
                {
                    evaluatedArgs[i] = evalResult.Value.RawValue;
                }
                else
                {
                    evaluatedArgs[i] = null;
                }
            }
            object? functionResult = function(evaluatedArgs);
            return new EvaluationResult(ResoltionState.Found, functionResult.AsFacade());
        }
        return new EvaluationResult(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitIdenitifer(IdentifierExpressionNode node, DataContext args)
    {
        EvaluationResult result = node.Path.Evaluate(accessorVisitor, args);
        return result;
    }

    public EvaluationResult VisitLiteral(LiteralExpressionNode node, DataContext _)
    {
        return new EvaluationResult(ResoltionState.Found, node.Constant.AsFacade());
    }

    public EvaluationResult VisitIndex(NumberExpressionNode node, DataContext _)
    {
        return new EvaluationResult(ResoltionState.Found, node.Constant.AsFacade());
    }
}
