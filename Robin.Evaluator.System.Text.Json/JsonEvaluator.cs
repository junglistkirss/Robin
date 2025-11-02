using Robin.Abstractions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;

namespace Robin.Evaluator.System.Text.Json;

public sealed class JsonEvaluator : IEvaluator
{
    public static readonly JsonEvaluator Instance = new();
    private static readonly ExpressionNodeVisitor NodeInstance = new(JsonAccesorVisitor.Instance);

    public IDataFacade Resolve(IExpressionNode expression, DataContext? data)
    {
        if (data is null)
            return DataFacade.Null;

        EvaluationResult result = expression.Accept(NodeInstance, data);

        if (result.Status == ResoltionState.NotFound && data.Parent is not null)
            result = expression.Accept(NodeInstance, data.Parent);

        if (result.Status == ResoltionState.Found)
            return result.Value;

        return DataFacade.Null;
    }
}

