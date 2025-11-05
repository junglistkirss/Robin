using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

internal sealed class ServiceEvaluator(IExpressionNodeVisitor<DataContext> visitor) : IEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        if (data is not null)
        {
            bool resolved = expression.Accept(visitor, data, out object? value);
            if (resolved)
            {
                facade = value.GetFacade();
                return value;
            }

            if (!resolved && data.Parent is not null)
            {
                resolved = expression.Accept(visitor, data.Parent, out object? parentValue);
                if (resolved)
                {
                    facade = parentValue.GetFacade();
                    return parentValue;
                }
            }
        }
        facade = DataFacade.Null;
        return null;
    }
}
