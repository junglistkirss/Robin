using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Facades;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using Robin.Nodes;

namespace Robin.Abstractions;

public sealed class ExpressionNodeVisitor(IVariableSegmentVisitor<EvaluationResult, SourceContext> accessorVisitor) : IExpressionNodeVisitor<EvaluationResult, SourceContext>
{
    public EvaluationResult VisitFunctionCall(FunctionCallNode node, SourceContext args)
    {
        //string funcName = args.GetValue(node.FunctionName);
        //if (args.Helper.TryGetFunction(funcName, out Helper.Function? function) && function is not null)
        //{
        //    object?[] evaluatedArgs = new object?[node.Arguments.Length];
        //    for (int i = 0; i < node.Arguments.Length; i++)
        //    {
        //        EvaluationResult evalResult = node.Arguments[i].Accept(this, args);
        //        if (evalResult.IsResolved)
        //        {
        //            evaluatedArgs[i] = evalResult.Value;
        //        }
        //        else
        //        {
        //            evaluatedArgs[i] = null;
        //        }
        //    }
        //    object? functionResult = function(evaluatedArgs);
        //    return new EvaluationResult(true, functionResult);
        //}
        return new EvaluationResult(false, null);
    }

    public EvaluationResult VisitIdenitifer(IdentifierExpressionNode node, SourceContext args)
    {
        if (node.Path.Evaluate(accessorVisitor, args, out object? value))
            return new EvaluationResult(true, value);
        return new EvaluationResult(false, null);
    }

    public EvaluationResult VisitLiteral(LiteralExpressionNode node, SourceContext _)
    {
        return new EvaluationResult(true, node.Constant);
    }

    public EvaluationResult VisitIndex(NumberExpressionNode node, SourceContext _)
    {
        return new EvaluationResult(true, node.Constant);
    }
}
