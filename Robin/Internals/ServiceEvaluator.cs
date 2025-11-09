using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;

namespace Robin.Internals;

internal sealed class ServiceEvaluator(IExpressionNodeVisitor<DataContext> visitor, IEnumerable<IDataFacadeResolver> facadeResolver) : IEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        if (data is not null)
        {
            if (expression.Accept(visitor, data, out object? value))
            {
                foreach (IDataFacadeResolver resolver in facadeResolver)
                {
                    if (resolver.ResolveDataFacade(value, out IDataFacade? resolvedFacade))
                    {
                        facade = resolvedFacade;
                        return value;
                    }
                }
                facade = value.GetPrimitiveFacade();
                return value;
            }
            else if (data.Parent is not null)
            {
                if (expression.Accept(visitor, data.Parent, out object? parentValue))
                {
                    foreach (IDataFacadeResolver resolver in facadeResolver)
                    {
                        if (resolver.ResolveDataFacade(parentValue, out IDataFacade? resolvedFacade))
                        {
                            facade = resolvedFacade;
                            return parentValue;
                        }
                    }
                    facade = parentValue.GetPrimitiveFacade();
                    return parentValue;
                }
            }
        }
        facade = DataFacade.Null;
        return null;
    }
}
