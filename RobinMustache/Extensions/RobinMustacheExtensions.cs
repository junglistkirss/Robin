using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions;
using RobinMustache.Abstractions.Accessors;
using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Expressions;
using RobinMustache.Abstractions.Facades;
using RobinMustache.Abstractions.Nodes;
using RobinMustache.Abstractions.Variables;
using RobinMustache.Internals;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

namespace RobinMustache.Extensions;

public static class RobinMustacheExtensions
{
    internal static Dictionary<string, ImmutableArray<INode>> ExtractsPartials(this ReadOnlySpan<INode> nodes, ReadOnlyDictionary<string, ImmutableArray<INode>>? baseCollection = null)
    {
        Dictionary<string, ImmutableArray<INode>> collection;
        if (baseCollection is not null)
            collection = new(baseCollection);
        else collection = [];
        foreach (INode node in nodes)
        {
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
            .AddSingleton<IPartialLoader, DefinedPartialLoader>()
            .AddSingleton<INodeVisitor<RenderContext<StringBuilder>>, StringNodeRender>()
            .AddSingleton<IVariableSegmentVisitor<Type, ChainableGetter>, ServiceAccesorVisitor>()
            .AddSingleton<IVariableSegmentVisitor<Type, ChainableGetter>, ServiceDelegateAccesorVisitor>();
    }
}

