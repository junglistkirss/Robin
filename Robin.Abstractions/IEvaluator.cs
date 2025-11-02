using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;

namespace Robin.Abstractions;


public interface IEvaluator
{
    IDataFacade Resolve(IExpressionNode expression, DataContext? data);
}



