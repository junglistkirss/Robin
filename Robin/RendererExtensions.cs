using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Nodes;

namespace Robin;

public static class RendererExtensions
{
    public static IRenderer ToRenderer(this INodeVisitor<NoValue, RenderContext> visitor, IEvaluator evaluator, Action<Helper>? helperConfig = null)
    {
        return new RendererImpl(visitor, evaluator, helperConfig);
    }

    public static IServiceCollection AddRenderer(
        this IServiceCollection services,
        Func<IServiceProvider, IEvaluator> evaluatorProvider,
        Func<IServiceProvider, INodeVisitor<NoValue, RenderContext>> visitorProvider,
        Action<Helper>? helperConfig = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.Add(new ServiceDescriptor(typeof(IRenderer), factory : sp =>
        {
            IEvaluator evaluator = evaluatorProvider(sp);
            INodeVisitor<NoValue, RenderContext> visitor = visitorProvider(sp);
            return new RendererImpl(visitor, evaluator, helperConfig);
        }, serviceLifetime));
        return services;
    }

    public static IServiceCollection AddKeyedRenderer(
       this IServiceCollection services,
       object? key,
       Func<IServiceProvider, object?, IEvaluator> evaluatorProvider,
       Func<IServiceProvider, object?, INodeVisitor<NoValue, RenderContext>> visitorProvider,
       Action<Helper>? helperConfig = null,
       ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.Add(new ServiceDescriptor(typeof(IRenderer), serviceKey: key, factory: (sp, k) =>
        {
            IEvaluator evaluator = evaluatorProvider(sp,k);
            INodeVisitor<NoValue, RenderContext> visitor = visitorProvider(sp, k);
            return new RendererImpl(visitor, evaluator, helperConfig);
        }, serviceLifetime));
        return services;
    }
}
