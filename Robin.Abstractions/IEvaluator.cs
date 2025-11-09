using Robin.Abstractions.Context;
using Robin.Abstractions.Expressions;
using Robin.Abstractions.Facades;

namespace Robin.Abstractions;

public interface IEvaluator
{
    object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade);
}
