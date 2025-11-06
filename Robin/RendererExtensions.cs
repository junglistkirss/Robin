using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Nodes;
using Robin.Internals;
using System.Text;

namespace Robin;

public static class RendererExtensions
{
    public static IRenderer<TOut> ToRenderer<T, TOut>(
        this T builder,
        Func<T, TOut> output,
        INodeVisitor<NoValue, RenderContext<T>> visitor,
        IEvaluator evaluator,
        Action<Helper>? helperConfig = null)
        where T : class
        => new RendererImpl<T, TOut>(builder, output, visitor, evaluator, helperConfig);

    public static IServiceCollection AddStringRenderer(
        this IServiceCollection services,
        Func<IServiceProvider, IEvaluator>? evaluatorProvider = null,
        Action<Helper>? helperConfig = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        services.Add(new ServiceDescriptor(typeof(IStringRenderer), typeof(StringRendererImpl), serviceLifetime));
        return services.AddRenderer(_ => new StringBuilder(), sb => sb.ToString(), evaluatorProvider ?? (s => s.GetRequiredService<IEvaluator>()), _ => StringNodeRender.Instance, helperConfig, serviceLifetime);
    }

    public static IServiceCollection AddKeyedStringRenderer(
        this IServiceCollection services,
       object? key,
        Func<IServiceProvider, object?, IEvaluator>? evaluatorProvider = null,
        Action<Helper>? helperConfig = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        services.Add(new ServiceDescriptor(typeof(IStringRenderer), key, typeof(StringRendererImpl), serviceLifetime));
        return services.AddKeyedRenderer(key, (_, _) => new StringBuilder(), sb => sb.ToString(), evaluatorProvider ?? ((s, _) => s.GetRequiredService<IEvaluator>()), (_, _) => StringNodeRender.Instance, helperConfig, serviceLifetime);
    }

    public static IServiceCollection AddRenderer<T, TOut>(
        this IServiceCollection services,
        Func<IServiceProvider, T> builderFactory,
        Func<T, TOut> output,
        Func<IServiceProvider, IEvaluator> evaluatorProvider,
        Func<IServiceProvider, INodeVisitor<NoValue, RenderContext<T>>> visitorProvider,
        Action<Helper>? helperConfig = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where T : class
    {
        services.Add(new ServiceDescriptor(typeof(IRenderer<TOut>), factory: sp =>
        {
            T builder = builderFactory(sp);
            IEvaluator evaluator = evaluatorProvider(sp);
            INodeVisitor<NoValue, RenderContext<T>> visitor = visitorProvider(sp);
            return builder.ToRenderer(output, visitor, evaluator, helperConfig);
        }, serviceLifetime));
        return services;
    }

    public static IServiceCollection AddKeyedRenderer<T, TOut>(
       this IServiceCollection services,
       object? key,
        Func<IServiceProvider, object?, T> builderFactory,
        Func<T, TOut> output,
       Func<IServiceProvider, object?, IEvaluator> evaluatorProvider,
       Func<IServiceProvider, object?, INodeVisitor<NoValue, RenderContext<T>>> visitorProvider,
       Action<Helper>? helperConfig = null,
       ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where T : class
    {
        services.Add(new ServiceDescriptor(typeof(IRenderer<TOut>), serviceKey: key, factory: (sp, k) =>
        {
            T builder = builderFactory(sp, k);
            IEvaluator evaluator = evaluatorProvider(sp, k);
            INodeVisitor<NoValue, RenderContext<T>> visitor = visitorProvider(sp, k);
            return builder.ToRenderer(output, visitor, evaluator, helperConfig);
        }, serviceLifetime));
        return services;
    }
}
