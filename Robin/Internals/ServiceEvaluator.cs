using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;

namespace Robin.Internals;

internal sealed class DataFacadeResolver(IServiceProvider provider, IMemoryCache cache) : IDataFacadeResolver
{

    private record DataFacadeCacheKey(Type ObjectType);
    public IDataFacade ResolveDataFacade(object? data)
    {
        if (data is null)
        {
            return DataFacade.Null;
        }
        Type type = data.GetType();
        return cache.GetOrCreate(new DataFacadeCacheKey(type), (_) =>
        {
            Type genType = typeof(IDataFacade<>).MakeGenericType(type);
            IDataFacade ? facade = (IDataFacade?)provider.GetService(genType);
            if(facade is not null) return facade;
            return data.GetFacade()!;
        }, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) }) ?? data.GetFacade();


    }
}

internal sealed class ServiceEvaluator(IExpressionNodeVisitor<DataContext> visitor, IDataFacadeResolver facadeResolver) : IEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        if (data is not null)
        {
            bool resolved = expression.Accept(visitor, data, out object? value);
            if (resolved)
            {
                facade = facadeResolver.ResolveDataFacade(value);
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
