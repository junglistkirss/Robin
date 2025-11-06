using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Context;
using Robin.Contracts.Expressions;
using Robin.Contracts.Nodes;
using Robin.Contracts.Variables;
using Robin.Internals;
using System.Collections.Immutable;

namespace Robin.Abstractions.Extensions;

public static class RobinExtensions
{
    public static ImmutableDictionary<string, ImmutableArray<INode>> ExtractsPartials(this IEnumerable<INode> nodes, ImmutableDictionary<string, ImmutableArray<INode>>? baseCollection = null)
    {
        return nodes.Aggregate(baseCollection ?? ImmutableDictionary<string, ImmutableArray<INode>>.Empty, (current, node) => node.Accept(PartialExtractor.Instance, current));
    }
    private const string BaseEvaluatorKey = "base";

    public static IServiceCollection AddServiceEvaluator(this IServiceCollection services)
    {
        return services
            .AddMemoryCache()
            .AddSingleton<IExpressionNodeVisitor<DataContext>, ExpressionNodeVisitor>()
            .AddKeyedSingleton<IEvaluator, ServiceEvaluator>(BaseEvaluatorKey)
            .AddSingleton<IEvaluator, ServiceEvaluator>()
            .AddSingleton<IVariableSegmentVisitor<Type>, ServiceAccesorVisitor>();
    }
}



