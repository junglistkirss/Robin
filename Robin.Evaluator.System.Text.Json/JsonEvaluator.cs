using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using System.Runtime.CompilerServices;

namespace Robin.Evaluator.System.Text.Json;

public sealed class JsonEvaluator : IEvaluator
{
    public static readonly JsonEvaluator Instance = new();
    private static readonly ServiceEvaluator BaseEvaluator = new(JsonAccesorVisitor.Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        object? value = BaseEvaluator.Resolve(expression, data, out IDataFacade baseFacade);
        facade = data.AsJsonFacade(baseFacade);
        return value;
    }
}

