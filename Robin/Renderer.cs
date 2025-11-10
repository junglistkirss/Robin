using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Abstractions.Helpers;
using Robin.Abstractions.Nodes;
using Robin.Internals;
using System.Text;

namespace Robin;

public static class Renderer
{

    public static IRenderer<TOut> CreateRenderer<TBuilder, TOut>(
        Func<TBuilder> builderFactory,
        Func<TBuilder, TOut> builderOutput,
        Func<IPartialLoader[], INodeVisitor<RenderContext<TBuilder>>> nodeVisitorFactory,
        Action<StaticDataFacadeResolverBuilder>? facades = null,
        Action<StaticAccessorBuilder>? accessors = null,
        Action<Helper>? helperConfig = null)
        where TBuilder : class
    {
        INodeVisitor<RenderContext<TBuilder>> nodeVisitor = nodeVisitorFactory([new DefinedPartialLoader()]);
        StaticDataFacadeResolverBuilder? facadesBuilder = new();
        if (facades is not null)
            facades(facadesBuilder);
        StaticDataFacadeResolver facadeResolver = facadesBuilder.Build();
        StaticAccessorBuilder accessorBuilder = new();
        if (accessors is not null)
            accessors(accessorBuilder);
        StaticServiceAccesorVisitor serviceAccesorVisitor = accessorBuilder.Build();
        ExpressionNodeVisitor nodeExpressionVisitor = new([serviceAccesorVisitor]);
        IEvaluator evaluator = new ServiceEvaluator(nodeExpressionVisitor, [facadeResolver]);
        return new RendererImpl<TBuilder, TOut>(
            builderFactory,
            builderOutput,
            nodeVisitor,
            evaluator,
            helperConfig);
    }
    public static IStringRenderer CreateStringRenderer(
        Action<StaticDataFacadeResolverBuilder>? facades = null,
        Action<StaticAccessorBuilder>? accessors = null,
        Action<Helper>? helperConfig = null)
    {
        IRenderer<string> impl = CreateRenderer(
            () => new StringBuilder(),
            x => x.ToString(),
             l => new StringNodeRender(l),
            facades,
            accessors,
            helperConfig);
        return new StringRendererImpl(impl);
    }
}
