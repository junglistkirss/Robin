using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Expressions;
using Robin.Abstractions.Facades;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonEvaluator(IEvaluator evaluator) : IJsonEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        object? value = evaluator.Resolve(expression, data, out IDataFacade baseFacade);
        facade = value.AsJsonFacade(baseFacade);
        return value;
    }
}
