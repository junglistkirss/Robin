using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

public sealed class ServiceEvaluator(IVariableSegmentVisitor<EvaluationResult, object?> accesorVisitor) : IEvaluator
{
    public IDataFacade Resolve(IExpressionNode expression, DataContext? data)
    {
        var visitor = new ExpressionNodeVisitor(accesorVisitor);
        if (data is null)
            return DataFacade.Null;

        EvaluationResult result = expression.Accept(visitor, data);

        if (result.Status == ResoltionState.NotFound && data.Parent is not null)
            result = expression.Accept(visitor, data.Parent);

        if (result.Status == ResoltionState.Found)
            return result.Value;

        return DataFacade.Null;
    }
}
