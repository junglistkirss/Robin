using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Accessors;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Nodes;
using Robin.Contracts.Variables;
using Robin.Internals;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Robin.Extensions;

public static class RobinExtensions
{
    public static Dictionary<string, ImmutableArray<INode>> ExtractsPartials(this ReadOnlySpan<INode> nodes, ReadOnlyDictionary<string, ImmutableArray<INode>>? baseCollection = null)
    {
        Dictionary<string, ImmutableArray<INode>> collection;
        if (baseCollection is not null)
            collection = new(baseCollection);
        else collection = [];
        foreach (INode node in nodes) {
            node.Accept(PartialExtractor.Instance, collection);
        }
        return collection;
    }
    public static IServiceCollection AddServiceEvaluator(this IServiceCollection services)
    {
        return services
            .AddSingleton<IExpressionNodeVisitor<DataContext>, ExpressionNodeVisitor>()
            // .AddKeyedSingleton<IEvaluator, ServiceEvaluator>(BaseEvaluatorKey)
            .AddSingleton<IDataFacadeResolver, DataFacadeResolver>()
            .AddSingleton<IEvaluator, ServiceEvaluator>()
            .AddSingleton<IVariableSegmentVisitor<Type, ChainableGetter>, ServiceAccesorVisitor>()
            .AddSingleton<IVariableSegmentVisitor<Type, ChainableGetter>, ServiceDelegateAccesorVisitor>();
    }
}



