using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using Robin.Nodes;

namespace Robin.Abstractions;

public sealed class ServiceEvaluator(IVariableSegmentVisitor<EvaluationResult, SourceContext> accesorVisitor) : IEvaluator
{
    private readonly ExpressionNodeVisitor visitor = new(accesorVisitor);
    public object? Resolve(IExpressionNode expression, SourceContext args, out IDataFacade facade)
    {
       
            EvaluationResult result = expression.Accept(visitor, args);

            if (!result.IsResolved && args.Data.Parent is not null)
            {
                SourceContext ctxParent = args with { Data = args.Data.Parent };
                result = expression.Accept(visitor, ctxParent);
            }

            if (result.IsResolved)
            {
                facade = result.Value.GetFacade();
                return result.Value;
            }
        facade = DataFacade.Null;
        return null;
    }
}
