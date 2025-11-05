using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using System.Runtime.CompilerServices;

namespace Robin.Evaluator.System.Text.Json;

public interface IJsonEvaluator : IEvaluator { }

internal sealed class JsonEvaluator(ServiceEvaluator evaluator) : IEvaluator, IJsonEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        object? value = evaluator.Resolve(expression, data, out IDataFacade baseFacade);
        facade = value.AsJsonFacade(baseFacade);
        return value;
    }
}

