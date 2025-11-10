using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Expressions;
using RobinMustache.Abstractions.Facades;

namespace RobinMustache.Abstractions;

public interface IEvaluator
{
    object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade);
}
