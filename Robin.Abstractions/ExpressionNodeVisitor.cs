using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Facades;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

public sealed class ExpressionNodeVisitor(IVariableSegmentVisitor<EvaluationResult, object?> accessorVisitor) : IExpressionNodeVisitor<EvaluationResult, DataContext>
{
    public EvaluationResult VisitFunctionCall(FunctionCallNode node, DataContext args)
    {
        if (args.Helper.TryGetFunction(node.FunctionName, out Helper.Function? function) && function is not null)
        {
            object?[] evaluatedArgs = new object?[node.Arguments.Length];
            for (int i = 0; i < node.Arguments.Length; i++)
            {
                EvaluationResult evalResult = node.Arguments[i].Accept(this, args);
                if (evalResult.IsResolved)
                {
                    evaluatedArgs[i] = evalResult.Value;
                }
                else
                {
                    evaluatedArgs[i] = null;
                }
            }
            object? functionResult = function(evaluatedArgs);
            return new EvaluationResult(true, functionResult);
        }
        return new EvaluationResult(false, null);
    }

    public EvaluationResult VisitIdenitifer(IdentifierExpressionNode node, DataContext args)
    {
        if (node.Path.Evaluate(accessorVisitor, args, out object? value))
            return new EvaluationResult(true, value);
        return new EvaluationResult(false, null);
    }

    public EvaluationResult VisitLiteral(LiteralExpressionNode node, DataContext _)
    {
        return new EvaluationResult(true, node.Constant);
    }

    public EvaluationResult VisitIndex(NumberExpressionNode node, DataContext _)
    {
        return new EvaluationResult(true, node.Constant);
    }
}
