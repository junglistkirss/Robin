using RobinMustache.Abstractions;
using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Expressions;
using RobinMustache.Abstractions.Facades;

namespace RobinMustache.Evaluator.System.Text.Json;

internal sealed class JsonEvaluator(IEvaluator evaluator) : IJsonEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        object? value = evaluator.Resolve(expression, data, out IDataFacade baseFacade);
        facade = value.AsJsonFacade(baseFacade);
        return value;
    }
}
